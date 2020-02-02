/**
this script will crop the active document with unit X unit
and export to $size x $size png with the document name_i.png to $saveFolder 
for it works, better set unit = size
*/

var debugCrop = false;
var sizeo = 256;
var size = new UnitValue(sizeo, 'px');
var unito = 256;
var unit = new UnitValue(unito, 'px');
var saveFolder = "<your path>/SaveTheEgg/src/VolitantEgg/Assets/Resources/Img";
//for debug
//var saveFolder = "/Users/kame/Desktop";
//the trunk total height
var trunkHeight = new UnitValue(1536, 'px');
var unityImportPPU = 100;
//unity screen width in world unit
var screenUnit = 5.2;
var rightTrunkUnityPos = 3.6;
//should be abs(unity pos)
var leftTrunkUnityPos = 3.5;



//end setting

//start inner var
//set the ruler type
if (app.preferences.rulerUnits != Units.PIXELS) {
    app.preferences.rulerUnits = Units.PIXELS;
}
var docRef = app.activeDocument;

//start main

//exportLeftTrunk();

//duplicate();

//setRightScreenGuid();

//setLeftScreenGuid();

//exportTrim(sizeo);
//for bg unit
//exportSelectedLayers(docRef, 2);
//for O1,O4,ui-ink-bar,bg-clouds,ui-progress，ico,leaf
//exportSelectedLayers(docRef, 64);
//for O3,E7-other,eggg
//exportSelectedLayers(docRef, 128);
//for E7-vine,main menu,ico
//exportSelectedLayers(docRef, 512);
//app ico
exportSelectedLayers(docRef, 1024);
//for E1,butterfly
//exportSelectedLayers(docRef, 64,false);
//for E4,E2,E5,E9,E7,E6,E8,egg-hatch,egg-gameover,egg-congra
//exportSelectedLayers(docRef, 128,false);
//for E4-nest
//exportSelectedLayers(docRef, 128);
//exportSelectedLayers(docRef, sizeo);




//start function
function duplicate() {
    var doc = docRef.duplicate();
}

function exportLeftTrunk() {
    exportUnit(unit, trunkHeight, 0);
}

function exportRightTrunk() {
    //TODO right tree, left start position
    exportUnit(unit, trunkHeight, 0);
}

function setLeftScreenGuid() {

    clearAllGuides();
    var trunkHalfWidth = sizeo * 0.5;
    var mid = trunkHalfWidth + getUnityPix(leftTrunkUnityPos);
    var left = mid - getUnityPix(screenUnit * 0.5);
    var right = mid + getUnityPix(screenUnit * 0.5);

    docRef.guides.add(Direction.VERTICAL, new UnitValue(left, 'px'));
    docRef.guides.add(Direction.VERTICAL, new UnitValue(right, 'px'));
    docRef.guides.add(Direction.VERTICAL, new UnitValue(mid, 'px'));
}
function setRightScreenGuid() {
    clearAllGuides();
    var trunkHalfWidth = sizeo * 0.5;
    var mid = docRef.width - trunkHalfWidth - getUnityPix(rightTrunkUnityPos);
    var left = mid - getUnityPix(screenUnit * 0.5);
    var right = mid + getUnityPix(screenUnit * 0.5);

    docRef.guides.add(Direction.VERTICAL, new UnitValue(left, 'px'));
    docRef.guides.add(Direction.VERTICAL, new UnitValue(right, 'px'));
    docRef.guides.add(Direction.VERTICAL, new UnitValue(mid, 'px'));
}

//this need you maintain the layer visibility at first manually
function exportTrim(sizeo) {
    size = new UnitValue(sizeo, 'px');
    var layer = docRef.activeLayer;
    var pname = layer.name;
    if (layer.parent) {
        pname = layer.parent.name + '-' + pname;
    }
    var path = saveFolder + "/" + pname + ".png";
    var duppedDocument = docRef.duplicate();

    try {
        //removeAllInvisibleArtLayers(duppedDocument);
        duppedDocument.trim(TrimType.TRANSPARENT);
        duppedDocument.resizeImage(size, size, null, ResampleMethod.AUTOMATIC);
        //$.writeln("w::"+duppedDocument.width+"---h::" + duppedDocument.height);
        exportPNG(duppedDocument, path);
    } finally {
        duppedDocument.close(SaveOptions.DONOTSAVECHANGES);
    }

}







//start utils funtions

/**
    get unity unit in the document pixel
*/
function getUnityPix(unit) {
    return unit * unityImportPPU;
}
/**
    document pixel to unity unit
*/
function getUnityUnit(px) {
    return px / unityImportPPU;
}

///////////////////////////////////////////////////////////////////////////////
// Function: setInvisibleAllArtLayers
// Usage: unlock and make invisible all art layers, recursively
// Input: document or layerset
// Return: all art layers are unlocked and invisible
///////////////////////////////////////////////////////////////////////////////
function setInvisibleAllArtLayers(obj) {
    for (var i = 0; i < obj.artLayers.length; i++) {
        obj.artLayers[i].allLocked = false;
        obj.artLayers[i].visible = false;
    }
    for (var i = 0; i < obj.layerSets.length; i++) {
        setInvisibleAllArtLayers(obj.layerSets[i]);
    }
}



///////////////////////////////////////////////////////////////////////////////
// Function: removeAllInvisibleArtLayers
// Usage: remove all the invisible art layers, recursively
// Input: document or layer set
// Return: <none>, all layers that were invisible are now gone
///////////////////////////////////////////////////////////////////////////////
function removeAllInvisibleArtLayers(obj) {
    for (var i = obj.artLayers.length - 1; 0 <= i; i--) {
        try {
            if (!obj.artLayers[i].visible) {
                obj.artLayers[i].remove();
            }
        }
        catch (e) {
        }
    }
    for (var i = obj.layerSets.length - 1; 0 <= i; i--) {
        removeAllInvisibleArtLayers(obj.layerSets[i]);
    }
}


///////////////////////////////////////////////////////////////////////////////
// Function: removeAllEmptyLayerSets
// Usage: find all empty layer sets and remove them, recursively
// Input: document or layer set
// Return: empty layer sets are now gone
///////////////////////////////////////////////////////////////////////////////
function removeAllEmptyLayerSets(obj) {
    var foundEmpty = true;
    for (var i = obj.layerSets.length - 1; 0 <= i; i--) {
        if (removeAllEmptyLayerSets(obj.layerSets[i])) {
            obj.layerSets[i].remove();
        } else {
            foundEmpty = false;
        }
    }
    if (obj.artLayers.length > 0) {
        foundEmpty = false;
    }
    return foundEmpty;
}


///////////////////////////////////////////////////////////////////////////////
// Function: zeroSuppress
// Usage: return a string padded to digit(s)
// Input: num to convert, digit count needed
// Return: string padded to digit length
///////////////////////////////////////////////////////////////////////////////
function removeAllInvisible(docRef) {
    removeAllInvisibleArtLayers(docRef);
    removeAllEmptyLayerSets(docRef);
}

function clearAllGuides() {
    var desc = new ActionDescriptor();
    var ref = new ActionReference();
    ref.putEnumerated(charIDToTypeID("Gd  "), charIDToTypeID("Ordn"), charIDToTypeID("Al  "));
    desc.putReference(charIDToTypeID("null"), ref);
    executeAction(charIDToTypeID("Dlt "), desc, DialogModes.NO);
}


function exportPNG(docRef, path) {
    //avoid show export dialog when new File
    app.displayDialogs = DialogModes.NO;
    var pngSaveOptions = new PNGSaveOptions();
    pngSaveOptions.compression = 9;
    $.writeln("path:" + path);
    var file = new File(path);

    docRef.saveAs(file, pngSaveOptions, false, Extension.NONE);
    //var file = File.saveDialog("Export as PNG to...");
    //if (file && ((file.exists && confirm("Overwrite " + file +"?")) || !file.exists)) {
    //   docRef.saveAs(file, pngSaveOptions, false, Extension.NONE);
    //}
}

function exportCropAsPng(doc, path, l, t, r, b) {
    var docRef = doc.duplicate();
    docRef.crop([l, t, r, b]);
    docRef.resizeImage(size, size, null, ResampleMethod.AUTOMATIC);

    try {
        exportPNG(docRef, path);
    } finally {
        docRef.close(SaveOptions.DONOTSAVECHANGES);
    }
}


function exportUnit(unit, height, leftStart) {
    var width = unit;
    //$.writeln("doc width:"+width);
    clearAllGuides();
    //docRef.guides.clear();
    //add guid
    debugCrop && docRef.guides.add(Direction.VERTICAL, new UnitValue(0));
    debugCrop && docRef.guides.add(Direction.VERTICAL, new UnitValue(width));
    for (var i = 0; ; i++) {
        var h = i * width;
        //$.writeln(i + ":" + h + ":" + docRef.height)
        debugCrop && docRef.guides.add(Direction.HORIZONTAL, new UnitValue(h));
        var p = docRef.name.replace('.psd', '');
        p = p + "_" + i;
        p = saveFolder + "/" + p + ".png";
        $.writeln("name:" + p)
        exportCropAsPng(docRef, p, new UnitValue(leftStart), new UnitValue(h), new UnitValue(leftStart + width), new UnitValue(h + width));


        if (h >= height) {
            break;
        }
        //if last one did not fix one unit, ignore it
        if (h + width * 2 > height) {
            break;
        }
    }

}


//get Selected Layers
function cTID(s) { return app.charIDToTypeID(s); };
function sTID(s) { return app.stringIDToTypeID(s); };

function newGroupFromLayers(doc) {


    var desc = new ActionDescriptor();
    var ref = new ActionReference();
    ref.putClass(sTID('layerSection'));
    desc.putReference(cTID('null'), ref);
    var lref = new ActionReference();
    lref.putEnumerated(cTID('Lyr '), cTID('Ordn'), cTID('Trgt'));
    desc.putReference(cTID('From'), lref);
    executeAction(cTID('Mk  '), desc, DialogModes.NO);
};

function undo() {
    executeAction(cTID("undo", undefined, DialogModes.NO));
};

function getSelectedLayers(doc) {
    var selLayers = [];
    newGroupFromLayers();

    var group = doc.activeLayer;
    var layers = group.layers;

    for (var i = 0; i < layers.length; i++) {
        selLayers.push(layers[i]);
    }

    undo();

    return selLayers;
};

function cleanObj(obj) {
    for (var i = obj.artLayers.length - 1; 0 <= i; i--) {
        try {
            obj.artLayers[i].visible = false;
        }
        catch (e) {
            $.writeln("er:" + e);
        }
    }
    for (var i = obj.layerSets.length - 1; 0 <= i; i--) {
        try {
            obj.layerSets[i].visible = false;
        }
        catch (e) {
            $.writeln("er:" + e);
        }
    }

}

function iteralAllArtLayers(obj, fn) {
    for (var i = obj.artLayers.length - 1; 0 <= i; i--) {
        var l = obj.artLayers[i];

        fn && fn(l);
    }
    for (var i = obj.layerSets.length - 1; 0 <= i; i--) {
        var ls = obj.layerSets[i];

        fn && fn(ls);
        iteralAllArtLayers(ls, fn);
    }
}

function iteralParents(obj, fn) {
    var p = obj.parent;
    while (p) {
        fn && fn(p);
        p = p.parent;
    }
}

function makeSureVisible(obj) {
    iteralParents(obj, function (p) {
        try {
            p.visible = true;
        }
        catch (e) {
            $.writeln("er:" + e);
        }
    });
}

function exportLayer(doc, layer, sizeo, trim) {
    //$.writeln('doc  ' + doc.typename);
    if (undefined != layer.layers) {
        //it is layers
        for (var index = 0; index < layer.layers.length; index++) {
            var l = layer.layers[index];
            if (undefined != l.layers) {
                //do not do recursion
                return;
            } else {
                exportLayer(doc, l, sizeo, trim);
            }
        }
        return;
    }
    size = new UnitValue(sizeo, 'px');
    var pname = layer.name;
    if (layer.parent) {
        pname = layer.parent.name + '-' + pname;
    }
    var path = saveFolder + "/" + pname + ".png";
    layer.visible = true;
    var lname = getLayerFullName(layer);
    //$.writeln('>>>>>>  ' + lname);
    layer.copy();
    var duppedDocument = docRef.duplicate();

    try {
        var nl = null;
        iteralAllArtLayers(duppedDocument, function (l) {
            try {
                if (lname == getLayerFullName(l)) {
                    nl = l;
                } else {
                    l.visible = false;
                }
            }
            catch (e) {
                $.writeln("er:" + e);
            }
        });
        makeSureVisible(nl);

        if (trim) {
            //$.writeln('will trim');
            duppedDocument.trim(TrimType.TRANSPARENT);
        }
        var thisunit = Math.max(duppedDocument.width, duppedDocument.height);
        duppedDocument.resizeCanvas(thisunit, thisunit);

        duppedDocument.resizeImage(size, size, null, ResampleMethod.AUTOMATIC);
        //$.writeln("w::"+duppedDocument.width+"---h::" + duppedDocument.height);
        exportPNG(duppedDocument, path);
    } finally {
        duppedDocument.close(SaveOptions.DONOTSAVECHANGES);
    }
}

//export the selected layers or one depth children of the selected layersets
//regardless they are visible or not
function exportSelectedLayers(doc, sizeo, trim) {
    trim = !(false === trim);
    $.writeln('trim is ' + trim);
    var lays = getSelectedLayers(doc);
    for (var i = 0; i < lays.length; i++) {
        var l = lays[i];
        exportLayer(doc, l, sizeo, trim);
    }
}

function getLayerFullName(l) {
    var p = l.parent;
    var n = '';
    while (null != p && p.typename != 'Document') {
        n = p.name + '-' + n;
        p = p.parent;
    }

    if (n) {
        n = n + l.name;
    } else {
        n = l.name;
    }

    return n;
}










