// uncomment to suppress Illustrator warning dialogs
// app.userInteractionLevel = UserInteractionLevel.DONTDISPLAYALERTS;
var G_desFolder = '<your path>/SaveTheEgg/src/SaveTheEgg/Assets/Resources/Img';
var size = 256;



//inner var 
var activeDocument = app.activeDocument;



//main
//exportLayer(activeDocument.activeLayer,size);

//E10
exportLayers(128);


//function start



//utils fucntion start
function exportLayer(layer, size) {
	var pname = layer.name + '.png';
	if (layer.parent) {
		pname = layer.parent.name + '-' + pname;
	}
	pname = G_desFolder + '/' + pname;
	var targetFile = this.getTargetFile(pname);

	var options = this.getOptions(size);
	activeDocument.exportFile(targetFile, ExportType.PNG24, options);
}


function exportLayers(size) {
	try {
		if (app.activeDocument && app.activeDocument.activeLayer) {
			var layP = app.activeDocument.activeLayer;
			// Get the folder to save the files into
			var destFolder = G_desFolder;

			if (destFolder != null) {
				var options, i, sourceDoc, targetFile;


				// Get the SVG options to be used.
				options = this.getOptions(size);

				// You can tune these by changing the code in the getOptions() function.

				for (i = 0; i < layP.layers.length; i++) {
					sourceDoc = layP.layers[i];

					sourceDoc.visible = false;
				}

				var preLayer = null;
				for (i = 0; i < layP.layers.length; i++) {
					if (null != sourceDoc) {
						sourceDoc.visible = false;
					}
					sourceDoc = layP.layers[i]; // returns the document object
					sourceDoc.visible = true;
					preLayer = sourceDoc;
					var llname = getLayerFullName(sourceDoc);
					$.writeln('name ' + llname);
					var pname = G_desFolder + '/' + llname + '.png';
					// Get the file to save the document as svg into
					targetFile = this.getTargetFile(pname);

					// Save as SVG
					activeDocument.exportFile(targetFile, ExportType.PNG24, options);
					$.writeln('export ' + pname);
					// Note: the doc.exportFile function for SVG is actually a Save As
					// operation rather than an Export, that is, the document's name
					// in Illustrator will change to the result of this call.				
				}
				//alert( 'Exports Done!' );
			}
		}
		else {
			throw new Error('There are no layer selected!');
		}
	}
	catch (e) {
		alert(e.message, "Script Alert", true);
	}

}


/** Returns the options to be used for the generated files.
	@return ExportOptionsSVG object
*/
function getOptions(size) {
	// Create the required options object
	var options = new ExportOptionsPNG24();
	var activeIndex = app.activeDocument.artboards.getActiveArtboardIndex();
	var activeArtboard = app.activeDocument.artboards[activeIndex];
	var vscale = size / Math.abs(activeArtboard.artboardRect[3] - activeArtboard.artboardRect[1]) * 100;
	var hscale = size / Math.abs(activeArtboard.artboardRect[2] - activeArtboard.artboardRect[0]) * 100;
	options.verticalScale = vscale;
	options.horizontalScale = hscale;
	options.antiAliasing = true;
	options.transparency = true;
	options.artBoardClipping = true;
	//alert(activeArtboard.artboardRect);

	return options;
}

function getTargetFile(path) {
	// Create the file object to save to
	var myFile = new File(path);

	// Preflight access rights
	if (myFile.open("w")) {
		myFile.close();
	}
	else {
		throw new Error('Access is denied');
	}
	return myFile;
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
