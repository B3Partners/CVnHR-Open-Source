To add backgrounds and watermarks to uploaded images, set the path in the 
background/watermark attribute of the image element in Web.Backload.config
Background images are only used in resize modes with canvas (fit, place)

The provided background.png and watermark.png are examples only. You can 
create your own images. PNG images can have an opacity value lower than 1. 

Note: Background and watermark images are a Pro/Enterprise features, 
while background color is a Standard feature




Example:

<backload xsi:noNamespaceSchemaLocation="Config\Web.Backload.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:name="urn:backload-schema">

  <!-- Sets a resize mode, background, watermark and vertical watermark position -->
  <images resizeMode="fit" background="~/Backload/Imaging/background.png" watermark="~/Backload/Imaging/watermark.png" watermarkPosition="bottom" />

</backload>