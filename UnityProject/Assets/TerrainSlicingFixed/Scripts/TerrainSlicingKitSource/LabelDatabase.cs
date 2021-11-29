//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;

namespace TerrainSlicingKit
{
	public static class LabelDatabase
	{
		public static GUIContent autoFillLabel = new GUIContent("Terrains in Group", "Fill the fields below with all the " +
			"terrains in your terrain group, starting with the first terrain, and preceding in order from left to right, " +
			"then bottom to top.\n\nPress the 'Auto Fill From Scene' button to have the script try and automatically " +
			"fill these fields in for you.");
			
		public static GUIContent autoSmoothLabel = new GUIContent("Smooth Edges", "If checked, the tiler will attempt to " +
			"smooth out the edges between neighboring terrains when you press the 'Make HeightMap Tileable' button." +
			"\n\nIf you don't like the results of smoothing, you can undo the tiling operation, manually smooth out " +
			"the edges, and redo the tiling operation.");
			
		public static GUIContent autoTileAlphaLabel = new GUIContent("Automatically Tile AlphaMap", "If checked, the " +
			"tiler will automatically tile the alpha maps (splat textures) of your terrains when you press the " +
			"'Make HeightMap(s) Tileable' button. You can leave this unchecked and use the 'Make AlphaMap(s) Tileable' " +
			"button later if you prefer.\n\nIn most cases you will probably "+
			"want/need to have your alpha maps tiled.");
			
		public static GUIContent baseTerrainLabel = new GUIContent("Base Terrain in Group", "The first terrain is the " +
			"terrain with the smallest x and z value for its position among the terrains in your group. From a top " +
			"down view, it should be on the bottom left-most terrain.");
			
		public static GUIContent columnsLabel = new GUIContent("Columns", "This value represents the number of terrains " +
			"that exist in a single row (along x axis) of your terrain group.");
									
		public static GUIContent copyAllDetailLabel = new GUIContent("Copy All Detail Meshes","Same deal as the trees. " +
			"Leave checked to copy every detail mesh to the new cut, or uncheck to only copy those detail meshes that fall within the cuts bounds.");
			
		public static GUIContent copyAllTreesLabel = new GUIContent("Copy All Trees","The base terrain contains a list of trees " +
			"which you can paint on it. By default the program will copy every tree from this list to the new terrain piece " +
			"you cut, regardless of whether that terrain cut currently has that tree painted on it.\n\nIf you want the newly " +
			"created terrain cut to have the full list of trees from the base terrain, leave this box checked.\n\n" +
			"However, if you would rather copy only those trees which the terrain cut has painted on it, uncheck this box."+
			"\n\nRegardless of the option you choose, all visible trees within the cut area on your base terrain will be " +
			"copied to the new terrain accurately.");
			
		public static GUIContent cutTerrainButtonLabel = new GUIContent("Cut Terrain", "Cut out the terrain area.");
			
		public static GUIContent decreaseCutSizeButtonLabel = new GUIContent("Decrease Cut Size", "Decrease the size of " +
			"the area to be cut, down to a minimum size.\n\nAs a general rule, the larger your base terrain's resolutions, " +
			"the smaller the cut size can be.");
			
		public static GUIContent desiredCutPatternLabel = new GUIContent("Desired Cut Pattern", "How many slices do you " +
			"want to cut the terrain into?\n\nEx: 2x2 means the terrain will be cut in half along both axises, " +
			"and 4 slices will be created.");			
			
		public static GUIContent detailResolutionPerPatchLabel = new GUIContent("Detail Res Per Patch", "The detail " +
			"resolution per patch value of the base terrain you're cutting/slicing. You need to enter it because " +
			"it's impossible to get via code.");
			
		public static GUIContent dimensionsLabel = new GUIContent("Slicing Dimensions","ex: 2 x 2 will divide terrain " +
			"by 2 along x axis and 2 along z axis, producing 4 terrain slices.\n\n4 x 4 will divide by 4, producing " +
			"16 terrain slices, and so on . . .");
			
		public static GUIContent drawPreviewButtonLabel = new GUIContent("Draw Preview Lines", "Enables or disables the preview.");
			
		public static GUIContent effectedRegionLabel = new GUIContent("Effected Region Width", "The edges of each terrain " +
			"will be adjusted so neighboring heightmap/alphamap points match exactly.\n\nIn addition, a portion of " +
			"each heightmap/alphamap inland from the edge must be adjusted to create a more natural transition between " +
			"the terrian edges. You can specifiy the width of that region here.\n\nA larger width will result in more " +
			"of the heightmap/alphamap being altered, but may produce more natural looking terrain.\n\nAs a general rule, " +
			"the closer your neighboring terrain edges are in height/color to each other (before tiling), the smaller " +
			"the effected region width will need to be.");
			
		public static GUIContent emptyLocationsLabel = new GUIContent("Empty Locations Exist", "Are there any empty " +
			"locations in your terrain group?\n\nExample: you have 2 rows and 2 columns, but terrain_1_2 does not " +
			"exist. If there are empty locations, in order to use this tool you will need to make sure all of your " +
			"terrains share a common base name and follow the naming convention 'name_row_column'. \n\nAlso be " +
			"aware that you cannot tile the inner edges of your terrain group if you have any empty locations.\n\n"+
			"You can still tile the outer edges (to make the world repeatable), but only if there are no empty locations " +
			"on the outer edge of your terrain group.");
			
		public static GUIContent fillFromSelectionsButtonLabel = new GUIContent("Fill From Selections", "Press this button " +
			"to fill the fields below with the selected game objects in the Hierarchy.");
			
		public static GUIContent gridHeightLabel = new GUIContent("Grid Height", "Change this value to move the preview grid up and down.");
			
		public static GUIContent increaseCutSizeButtonLabel = new GUIContent("Increase Cut Size", "Increase the size of " +
			"the area to be cut, up to a maximum of 1/2 the size of your terrain in both the x and z axis.");
			
		public static GUIContent objectsToConvertLabel = new GUIContent("Objects to Convert", "Fill the fields below " +
			"with the objects you wish to turn into prefabs.\n\nAlternatively, you can select the objects you wish to " +
			"turn into prefabs in the hierarchy, and press the 'Fill From Selections' button to have the script try and " +
			"automatically fill these fields in for you.");
			
		public static GUIContent onlyTileInnerLabel = new GUIContent("Only Tile Inner Edges", "Check this if you only want " +
			"to tile the inner edges of your terrain group. This will make your terrain slices match up, but your terrain " +
			"group as a whole will not be tiled with itself (you won't be able to have an endless world).\n\nThis will only " +
			"work if there are no empty terrains in your terrain group.");
			
		public static GUIContent onlyTileOuterLabel = new GUIContent("Only Tile Outer Edges", "Check this if you only " +
			"want to tile the outer edges of your terrain group. This will make your terrain group tileable  (so you " +
			"can have an enldess world), but your slices might not match up with each other.\n\nThis is only recommended " +
			"when your inner edges already match up and you have empty locations in one or more of the inner cells of " +
			"your terrain group. This will not work if you have empty locations in a cell along the outer region of your terrain group.");
						
			public static GUIContent overwriteDataButtonLabel = new GUIContent("Overwrite Terrain Data","The terrain data " +
			"names are derived from the base terrain's name, so if you try to slice a terrain with the same name as a " +
			"terrain that you've sliced in the past, you risk overwriting the existing terrain data.\n\nYou may wish " +
			"for this data to be overwritten, but to keep you from overwriting data on accident I've included this " +
			"checkbox field. By default it is unchecked, and the program will not let you overwrite data while it is " +
			"left unchecked. So if you intentionally want to overwrite data, check this box.");
			
		public static GUIContent overwritePrefabsLabel = new GUIContent("Overwrite Prefabs", "Check this if you wish " +
			"to automatically overwrite existing prefabs when the program tries to create a prefab of the same name " +
			"as one that already exist.\n\nIf left unchecked, the program will bring up a warning message for each " +
			"object that will overwrite an existing prefab, and you can choose to skip prefab generation for that " +
			"object if you don't wish for the existing prefab to be overwritten.");
			
		public static GUIContent pathToSavePrefabsLabel = new GUIContent("Save Prefabs @ File Path :", "This is the " +
			"location you wish to save the prefabs in, in relation to the Assets folder. If this directory doesn't " +
			"exist, it will be created.");
			
		public static GUIContent pathToStoreTerrainDataLabel = new GUIContent("Store Terrain Data @", "Where do you wish to " +
			"store the newly created terrain data? This path is in relation to the Assets folder, so leave 'Assets' out of the path name.");
			
		public static GUIContent prefabLoadPathLabel = new GUIContent("Use Prefabs @ File Path :", "This is the location where the " +
			"prefabs are stored, in relation to the Assets folder.\n\nExample: to get prefabs from Assets/Resources type /Resources");
			
		public static GUIContent recalculateDataButtonLabel = new GUIContent("Recalculate Data", "If, after adding this " +
			"script, you change the size or resolution data of the terrain this script is attached to, some variables " +
			"need to be recalculated.\n\nPress this button to perform these recalculations. If you haven't changed " +
			"anything except the position of the terrain, you don't need to use this button.");
			
		public static GUIContent resetFilePathButtonLabel = new GUIContent("Reset File Path to Default: " + 
			PlayerPrefs.GetString("File Path"),"This button simply resets the field above with the " +
			"default file path stored in player prefs (which you can change at any time by entering a new " +
			"file path above and clicking the button below this one).Use this if you make a mistake or need " +
			"to reset the file path to the default for any reason.");
			
		public static GUIContent rowsLabel = new GUIContent("Rows", "This value represents the number of terrains " +
			"that exist in a single column (along z axis) of your terrain group.");
			
		public static GUIContent saveFilePathButtonLabel = new GUIContent("Save Current File Path as Default File Path",
			"Click this if you want the file path entered above to be saved as the default file path." +
			"This will make this file path the default file path shown in the above field when you open the Terrain Slicing Tool.");
			
		public static GUIContent sharedNameLabel = new GUIContent("Shared Name","The common shared name of your prefabs, i.e., " +
			"'Terrain' if your prefabs are called 'Terrain_1_1', 'Terrain_1_2', etc.");
			
		public static GUIContent snapButtonLabel = new GUIContent("Snap to Common Point", "The bottom left point of the " +
			"cut area must begin on a point common to the different resolutions of the base terrain.\n\nBy default, the " +
			"cut area is moved to such a point automatically when you initiate a cut, but if you want to get a visual " +
			"representation of the true area that will be cut before cutting, use this button.");
			
		public static GUIContent terrainsInGroupLabel = new GUIContent("Terrains in Group", "Fill the fields below with " +
			"all the terrains in your terrain group, starting with the first terrain, and preceding in order from left " +
			"to right, then bottom to top.\n\nPress the 'Auto Fill From Scene' button to have the script try and " +
			"automatically fill these fields in for you.");
			
//			public static GUIContent cellObjectTypeLabel = new GUIContent("Using Non-Unity Terrain", "Check this if you are using something other than the built in Unity terrain, such as a mesh. SetNeighbors will not be called on non-Unity terrains.");
			
		public static GUIContent tileAlphaButtonLabel = new GUIContent("Make AlphaMap(s) Tileable", "Make the alphamaps " +
			"of this terrain group tileable. The terrain group as a whole will be made tileable with itself, unless " +
			"you check the 'Only Tile Inner Edges' check box.\n\nIf only a single terrain exist, it will be made tileable " +
			"with itself regardless of whether this check box is checked.");
			
		public static GUIContent tileHeightButtonLabel = new GUIContent("Make HeightMap(s) Tileable", "Make the " +
			"heightmaps of this terrain group tileable. The terrain group as a whole will be made tileable with itself, unless " +
			"you check the 'Only Tile Inner Edges' check box.\n\nIf only a single terrain exist, it will be made tileable " +
			"with itself regardless of whether this check box is checked.");
	}
}