# Atomic Map Editor XML Map Format

## &lt;map>
- **version**: *integer* - The version number of the file format.
- **name**: *string* - The name of the map.
- **author**: *string* - The creator of the map.
- **rows**: *integer* - The number of rows in tiles. This is analogous to the height.
- **columns**: *integer* - The number of columns in tiles. This is analogous to the width.
- **tileWidth**: *integer* - The default tile width in pixels.
- **tileHeight**: *integer* - The default tile height in pixels.
- **scale**: *double* - The ratio to increase the pixel size. 
- **backgroundColor**: *string* - The background of the map stored in a hex format (#RRGGBB). Optionally, an alpha value can be provided (#AARRGGBB).  
- **description**: *string* - An optional desciption of the map. 

Contains optional property and statistic fields

## &lt;tileset>
- **id**: *integer* - Unique identifier referenced by layers.
- **name**: *string* - The name of the tileset.
- **source**: *string* - The full file path to the tileset file.
- **isTransparent**: *boolean* - Flag indicating if the tileset has a transparent color.
- **transparentColor**: *string* - The transparent color of the tileset stored in a hex format (#RRGGBB). This field may have an alpha field (#AARRGGBB). The **isTransparent** color must be enabled for a transparent color to be applied.
- **width**: *integer* - The number of horizontal tiles in the tileset.
- **height**: *integer* - The number of vertical tiles in the tileset.
- **xOffset**: *integer* - The number of x pixels that the tileset begins on. The count begins from the left.
- **yOffset**: *integer* - The number of y pixels that the tileset begins on. The count begins from the top.
- **xPadding**: *integer* - The number of x pixels between tiles.
- **yPadding**: *integer* - The number of y pixels between tiles.
- **description**: *string* - An optional description of the tileset.

## &lt;layerGroup>
- **id**: *integer* - The unique identifier.
- **name**: *string* - The name of the layer.

## &lt;layer>
- **id**: *integer* - The unique identifier.
- **name**: *string* - The name of the layer.
- **group**: *integer* - If this layer belongs to a layer group, the identifier of the layer group.
- **rows**: *integer* - The number of rows in tiles. This is analogous to the height.
- **columns**:  *integer* - The number of columns in tiles. This is analogous to the width.
- **tileWidth** *integer* - The tile width in pixels.
- **tileHeight** *integer* - The tile height in pixels.
- **xOffset**: *integer* - The number of horizontal pixels that the layer begins on relative to the map. This count begins from the left.
- **yOffset**: *integer* - The number of vertical pixels that the layer beings on relative to the map. This count begins from the top.
- **position**: *string* - The default position of tiles relative to the player entity. There are three values for this: base, front, and back. Base renders all tiles on the same level as the player, front render all tiles in front of the player, and back renders all tiles behind the player.
- **scrollRate**: *double* - The scroll rate ratio of the tile relative to the player movement. A value of 2 would move the tiles 2 pixels for every pixel the player travels.
- **description**: *string* - An optional description of the layer.

Contains optional property and statistic fields

## &lt;tiles>
Specifies the list of tiles in a tileset id for a layer
- **id**: *integer* - Identifier referencing the tileset id
- **tilesetIds**: *list of integers* - Comma seperated list of ids referencing each tile ID in the tileset.
- **positions**: *list of integers* - Comma seperated list of position pairs (X, Y) of each tile