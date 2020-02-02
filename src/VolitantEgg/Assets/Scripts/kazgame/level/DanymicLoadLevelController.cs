using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using kazgame.utils;
using kazgame.gamesave;
using System;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace kazgame.level
{
    public class DanymicLoadLevelController : MonoBase, GameSaverable, PauseAble
    {
        struct SegmentKV
        {
            public LevelSegment level;
            public LevelSegmentMeta meta;
        };

        struct RequestKV
        {
            public LevelSegmentMeta meta;
            public ResourceRequest request;
        }

        [SerializeField]
        public List<LevelSegmentMeta> metas = null;
        public float loadSegmentLead = 5f;
        public const float forceLoadSegmentLead = 0.1f;
        public Vector3 prefabsPostion;
        public Camera traceCamera;
        public Direction2D direction2D;
        public Transform copyParent;

        List<SegmentKV> _loadedSegments = new List<SegmentKV>();
        List<SegmentKV> _needRemoveSegments = new List<SegmentKV>(5);
        List<RequestKV> _aysncLoadRequest = new List<RequestKV>(5);
        List<RequestKV> _needRemoveRequest = new List<RequestKV>(5);

        private bool _pausing;

        public bool pausing
        {
            get
            {
                return _pausing;
            }
        }

        protected override void DoInit()
        {
            base.DoInit();
            if (null == traceCamera)
            {
                traceCamera = Camera.main;
            }
            if (null == copyParent)
            {
                copyParent = transform;
            }
        }

        #region PauseAble implementation

        public virtual void OnPause()
        {
            _pausing = true;
            PauseChildren(true);
        }

        public virtual void OnResume()
        {
            _pausing = false;
            PauseChildren(false);
        }

        #endregion

        void PauseLevelSegment(LevelSegment l, bool pause)
        {
            PauseAble[] ps = l.GetComponentsInChildren<PauseAble>();
            if (null == ps)
            {
                return;
            }
            foreach (PauseAble item in ps)
            {
                if (pause)
                {
                    item.OnPause();
                }
                else
                {
                    item.OnResume();
                }
            }
        }

        void PauseChildren(bool pause)
        {
            foreach (SegmentKV skv in _loadedSegments)
            {
                LevelSegment l = skv.level;

                //				Log.Debug ("pause {0}",l.gameObject.name);

                PauseLevelSegment(l, pause);
            }
        }

        void Awake()
        {
            InitSelf();
        }

        void Update()
        {
            ManageSegment();
        }

        /// <summary>
        /// comming lead should be >=0.
        /// </summary>
        /// <returns>The comming lead.</returns>
        /// <param name="position">Position.</param>
        protected virtual float GetCommingLead(Vector2 position)
        {
            Vector3 mpoint = traceCamera.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            float lead = 0f;
            switch (direction2D)
            {
                case Direction2D.down:
                    {
                        lead = -(position.y - mpoint.y);
                        break;
                    }
                default:
                    throw new System.Exception(String.Format("have not implement for direction {0}", direction2D));
            }

            return lead;
        }

        /// <summary>
        ///  >=0 mean have leave the screen
        /// </summary>
        /// <returns>The leaving lead.</returns>
        /// <param name="position">Position.</param>
        /// <param name="length">Length.</param>
        protected virtual float GetLeavingLead(Vector2 position, float length)
        {
            float lead = 0f;
            switch (direction2D)
            {
                case Direction2D.down:
                    {
                        Vector3 ppoint = traceCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
                        lead = position.y - length - ppoint.y;
                        break;
                    }
                default:
                    throw new System.Exception(String.Format("have not implement for direction {0}", direction2D));
            }

            return lead;
        }

        protected virtual bool CheckUnload(LevelSegment l)
        {
            bool did = false;
            switch (direction2D)
            {
                case Direction2D.down:
                    {
                        Vector3 mpoint = traceCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
                        did = l.gameObject.transform.position.y - l.segmentLength > mpoint.y;
                        if (!did)
                        {
                            //check if load too far away from postion when camera move back
                            float lead = GetCommingLead(l.gameObject.transform.position);
                            if (lead > 2 * loadSegmentLead)
                            {
                                Log.Debug("unloaded too far. {0}", l);
                                did = true;
                            }
                        }
                        break;
                    }
                default:
                    throw new System.Exception(String.Format("have not implement for direction {0}", direction2D));
            }

            return did;
        }

        public void ManageSegment()
        {
            if (null == metas)
            {
                metas = new List<LevelSegmentMeta>();
            }
            // Log.Debug("ManageSegment {0} -- {1}", metas.Count, metaCount);
            //async loaded
            foreach (RequestKV rkv in _aysncLoadRequest)
            {
                ResourceRequest rr = rkv.request;
                if (!rr.isDone)
                {
                    continue;
                }
                _needRemoveRequest.Add(rkv);
                UnityEngine.Object obj = rr.asset;
                Log.Debug("{0} async load done {1}.", rkv.meta.prefPath, obj);

                //				LevelSegment lseg = Instantiate (obj) as LevelSegment;
                LoadedRsSegment(obj as LevelSegment, rkv.meta);
            }
            foreach (RequestKV l in _needRemoveRequest)
            {
                _aysncLoadRequest.Remove(l);
            }
            _needRemoveRequest.Clear();

            //start load
            foreach (LevelSegmentMeta meta in metas)
            {
                if (meta.state == LevelSegmentMeta.LoadState.loaded)
                {
                    // Log.Debug("haveloaded segment 1.1 {0}", meta);
                    continue;
                }
                float lead = GetCommingLead(meta.position);
                //				Log.Debug ("{0} lead is {1}",meta,lead);
                if (lead < 0)
                {
                    lead = GetLeavingLead(meta.position, meta.length);
                    if (lead >= 0)
                    {
                        //have left screen
                        continue;
                    }
                    else
                    {
                        //need force load
                        Log.Debug("force load segment 1 {0}", meta);
                        LevelSegment seg = Resources.Load<LevelSegment>(meta.prefPath);

                        LoadedRsSegment(seg, meta);
                    }
                }
                else if (Math.Abs(lead) < Mathf.Epsilon)
                {
                    //need force load
                    Log.Debug("force load segment 2 {0}", meta);
                    LevelSegment seg = Resources.Load<LevelSegment>(meta.prefPath);

                    LoadedRsSegment(seg, meta);
                }
                else if (meta.state == LevelSegmentMeta.LoadState.not || meta.state == LevelSegmentMeta.LoadState.unloaded)
                {
                    if (lead <= loadSegmentLead)
                    {
                        ResourceRequest req = Resources.LoadAsync<LevelSegment>(meta.prefPath);
                        RequestKV rkv = new RequestKV();
                        rkv.request = req;
                        rkv.meta = meta;
                        _aysncLoadRequest.Add(rkv);
                        meta.state = LevelSegmentMeta.LoadState.loading;
                    }
                }
                else if (meta.state == LevelSegmentMeta.LoadState.loading)
                {
                    //force load
                    if (lead <= forceLoadSegmentLead)
                    {

                        Log.Debug("force load segment 3 {0}", meta);
                        LevelSegment seg = Resources.Load<LevelSegment>(meta.prefPath);
                        //						seg = Instantiate (seg);

                        LoadedRsSegment(seg, meta);
                    }
                }
            }

            //remove
            foreach (SegmentKV skv in _loadedSegments)
            {
                LevelSegment l = skv.level;
                if (CheckUnload(l))
                {
                    Log.Info("segment {0}:{1} need unload.", l.gameObject.name, l.gameObject.transform.position);
                    _needRemoveSegments.Add(skv);
                }
            }

            bool needClean = _needRemoveSegments.Count > 0;
            foreach (SegmentKV l in _needRemoveSegments)
            {
                //				Resources.UnloadAsset (l.meta.loadedAsset);
                _loadedSegments.Remove(l);
                UnityEngine.Object.Destroy(l.level.gameObject);
                l.meta.state = LevelSegmentMeta.LoadState.unloaded;
                Log.Debug("UNLOAD SEGMENT:{0}", l.meta);
            }
            _needRemoveSegments.Clear();
            if (needClean)
            {
                Resources.UnloadUnusedAssets();
            }
        }



        #region GameSaverable implementation

        Hashtable GameSaverable.SaveGame()
        {
            Hashtable hs = new Hashtable();
            hs.Add("key1", "soem value");

            //TODO
            //			1: ant

            //			2: line
            //			3: segement 
            //			3.1: Segment collect all objects of interface saverable



            return hs;
        }

        void GameSaverable.LoadGame(Hashtable data)
        {
            Log.Info("GameSaverable.Load : {0}", data["key1"]);

            //TODO
            //			1: set ant
            //			2: create lines
            //			3: goto sgement
            //			3.1: set all the saverable objects


        }

        #endregion


        public Dictionary<string, GameSaverable> BuildSaverMap()
        {
            Dictionary<string, GameSaverable> dic = new Dictionary<string, GameSaverable>();

            dic.Add(GetType().FullName, this);

            //TODO
            //			1: ant
            //			dic.Add(LevelScene.getInstance().getAntManager().GetType().Name,LevelScene.getInstance().getAntManager());
            //			2: line
            //			dic.Add(LevelScene.getInstance().lineManager.GetType().Name,LevelScene.getInstance().lineManager);
            //			3: segement 
            //			3.1: Segment collect all objects of interface saverable


            return dic;
        }

        void LoadedRsSegment(LevelSegment lseg, LevelSegmentMeta meta)
        {
            if (meta.state == LevelSegmentMeta.LoadState.loaded)
            {
                Log.Debug("ignore lated segment load. {0}", meta);

                return;
            }
            lseg = Instantiate(lseg);
            meta.state = LevelSegmentMeta.LoadState.loaded;
            lseg.gameObject.transform.position = meta.position;
            lseg.gameObject.transform.SetParent(copyParent);
            SegmentKV skv = new SegmentKV();
            skv.level = lseg;
            skv.meta = meta;
            _loadedSegments.Add(skv);
            Log.Debug("LOAD SEGMENT:{0}", lseg);
        }

        public void ResetSegments()
        {
            // copyParent.
            GameObjectUtils.CleanChild(copyParent.gameObject);
            foreach (LevelSegmentMeta meta in metas)
            {
                meta.state = LevelSegmentMeta.LoadState.not;
            }
            _needRemoveSegments.Clear();
            _needRemoveSegments.AddRange(_loadedSegments);
            foreach (SegmentKV l in _needRemoveSegments)
            {
                Resources.UnloadAsset(l.level);
                _loadedSegments.Remove(l);
                Log.Debug("UNLOAD SEGMENT:{0}", l.meta);
            }
            _needRemoveSegments.Clear();
            // _needRemoveSegments = null;

            foreach (RequestKV rkv in _aysncLoadRequest)
            {
                ResourceRequest rr = rkv.request;
                if (rr.isDone)
                {
                    GameObject goo = (GameObject)rr.asset;
                    Resources.UnloadAsset(goo);

                }
                else
                {
                }
            }
            _aysncLoadRequest.Clear();
            // _aysncLoadRequest = new ArrayList();
        }

        void Destroy()
        {
            //remove
            ResetSegments();
        }

#if (UNITY_EDITOR)
        public int metaCount;
        public bool locked;
        public GameObject designLevel;
        public GameObject tmpCopyParent;
        public string autoSavePrefabsFolder;

        GameObject EdGetSyncLevelGroupObject()
        {
            return designLevel;
        }

        LevelSegment[] EdGetCorespondingSegments()
        {
            GameObject obj = EdGetSyncLevelGroupObject();
            if (null == obj)
            {
                return null;
            }
            Transform sgesobj = obj.transform;

            //			LevelSegment[] lsegs = sgesobj.gameObject.GetComponentsInChildren<LevelSegment> (true);
            LevelSegment[] lsegs = GameObjectUtils.GetComponentsInChildren<LevelSegment>(sgesobj.gameObject, false);

            return lsegs;
        }

        [ContextMenu("Collect Level Data")]
        public void EdCollectEditorLevelData()
        {
            if (null == metas)
            {
                metas = new List<LevelSegmentMeta>();
            }

            metas.Clear();
            LevelSegment[] lsegs = EdGetCorespondingSegments();

            if (null == lsegs)
            {
                Log.Debug(" no segments for level " + gameObject.name);
                return;
            }
            foreach (LevelSegment l in lsegs)
            {
                LevelSegmentMeta sm = new LevelSegmentMeta();
                sm.position = l.gameObject.transform.position;
                string path = autoSavePrefabsFolder + "/" + GameObjectUtils.GetUniqNameByHierarchy(l.gameObject);
                //string path = AssetsUtils.GetPrefabsResourceRelativePathWithoutExtention (l.gameObject);
                if (string.IsNullOrEmpty(path))
                {
                    Log.Error("the segment {0} is not a prefabs yet.", l);
                    continue;
                }
                sm.length = l.segmentLength;
                sm.prefPath = path;
                Log.Debug("{0} > {1}", l.gameObject.name, sm);
                metas.Add(sm);
            }
            metaCount = metas.Count;

            metas.Sort(CompareListByPosition);
            EdStore();
        }

        void EdStore()
        {
            prefabsPostion = gameObject.transform.position;
            EdSaveUnlinkPrefabs(gameObject);
        }

        private static int CompareListByPosition(LevelSegmentMeta i1, LevelSegmentMeta i2)
        {
            return (i1.position.x).CompareTo(i2.position.x);
        }

        [ContextMenu("Save Segment Prefabs")]
        public void EdSaveSegmentPrefabs()
        {
            LevelSegment[] lsegs = EdGetCorespondingSegments();

            if (null == lsegs)
            {
                return;
            }

            foreach (LevelSegment l in lsegs)
            {
                EdSaveUnlinkPrefabs(l.gameObject);
            }
        }

        private void EdSaveUnlinkPrefabs(GameObject go)
        {
            string path = autoSavePrefabsFolder + "/" + GameObjectUtils.GetUniqNameByHierarchy(go);
            Log.Debug("going to save to path " + path);
            AssetsUtils.SaveAsUnlinkPrefabsResourceRelativePathWithoutExtention(tmpCopyParent, path, go);
        }

#endif
    }
}
