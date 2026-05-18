import os
from PIL import Image, ImageDraw

output_path = r'w:\ASHU_tpi76_W\10_PROJECTS\Retaining Wall\RetainingWallSubassembly\deployment\RoadEdgeRetainingWall.bundle\Images\RoadEdgeRetainingWall.png'
os.makedirs(os.path.dirname(output_path), exist_ok=True)

# Create 64x64 transparent image
img = Image.new('RGBA', (64, 64), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Draw ground line (green/brown)
draw.rectangle([0, 48, 64, 64], fill=(139, 69, 19, 255))
draw.rectangle([0, 48, 64, 52], fill=(34, 139, 34, 255))

# Draw concrete wall (gray)
# Base
draw.rectangle([16, 40, 48, 48], fill=(169, 169, 169, 255), outline=(105, 105, 105, 255))
# Stem
draw.rectangle([24, 16, 32, 40], fill=(169, 169, 169, 255), outline=(105, 105, 105, 255))

# Draw some dimension lines or accents
draw.line([36, 16, 36, 40], fill=(255, 0, 0, 255), width=1)
draw.polygon([(36, 16), (34, 20), (38, 20)], fill=(255, 0, 0, 255))
draw.polygon([(36, 40), (34, 36), (38, 36)], fill=(255, 0, 0, 255))

img.save(output_path)
print(f"Generated icon at {output_path}")

surface_icon_path = r'w:\ASHU_tpi76_W\10_PROJECTS\Retaining Wall\RetainingWallSubassembly\deployment\RoadEdgeRetainingWall.bundle\Images\Surface.png'
# Create another for the surface command
img2 = Image.new('RGBA', (64, 64), (0, 0, 0, 0))
draw2 = ImageDraw.Draw(img2)
# Draw a surface grid
for i in range(0, 64, 8):
    draw2.line([0, i, 64, i+16], fill=(0, 100, 255, 100), width=1)
    draw2.line([i, 0, i+16, 64], fill=(0, 100, 255, 100), width=1)
draw2.polygon([(10, 32), (32, 10), (54, 32), (32, 54)], outline=(0, 0, 255, 255), width=2)
img2.save(surface_icon_path)
print(f"Generated icon at {surface_icon_path}")
