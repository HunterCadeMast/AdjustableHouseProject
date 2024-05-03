# AdjustableHouseProject

**Video Link:**
https://youtu.be/2a0xJKfd9jg

The folder "Final Project" contains the paper, video, and gallery of images taken of the project along with all of the targets used.

**Project Description:**

This project is a demo for an adjustable house designer in an augmented reality environment. The project allows a creation of an environment with an outer corner layout, opening layout, interior wall design, furniture placement, and color choice. The main goal of this demo was to have a better idea of tracking using the Vuforia package in the Unity game engine. With this, we will be able to have a better idea of how a house's layout options and the ability to change quickly. Tracking focuses on the aspects of Image targets are included in the file path "".

Main code lies in the "" path. "ModelCreator.cs" handles all target events. "ModelSwitch" handles all color menu events.

**Installation (Tested on Android):**
Using Android Device:
- Locate "BuildAndriod" file path.
- Connect device to computer.
- In file explorer, drag "" APK onto device.
- Open app to use project.

Using IOS Device:
- Locate "BuildIOS" file path.
- Connect device to computer.
- Using XCode, upload "" XCode file onto device.
- Open app to use project.

Vuforia Verion: 10.21.3 (May need to be installed seperately)
Unity Version: 3.7

2 different files had to be deleted to upload to GitHub (Will need to reinstall Vuforia to the project):
- \AdjustableHouseProject\BuildIOS\Frameworks\com.ptc.vuforia.engine\Vuforia\Plugins\iOS\Vuforia.framework\Vuforia
- \AdjustableHouseProject\BuildIOS/Data/sharedassets0.asset

**APK Tests:**
Test 1: Inner Acrossed and Next
Test 2: Inner and Opening
Test 3: Openings, Furniture, and Color Menu

For APKs, the main one has no test label. The rest in the folder, "Tests", handle these conditions. Due to this being so computationally expensive to run, I had to simulate most of them in Unity's environment. Tests were attempted, but crashed. Targets must manually be placed in Unity as GameObjects to the correct target list.

**Assets Used:**

- Wooden Floor Materials by Casual2D

- Sofa
https://www.homedepot.com/p/Harper-Bright-Designs-131-in-Square-Arm-3-Piece-Polyester-U-Shaped-Sectional-Sofa-in-Gray-with-Chaise-WYT104AAE/320923588#overlay

- Bed
https://www.target.com/p/costway-twin-size-upholstered-bed-frame-button-tufted-headboard-mattress-foundation-grey/-/A-87280287

- Door
https://www.homedepot.com/p/Steves-Sons-Regency-Collection-Customizable-Fiberglass-Front-Door-552299/311301192?55419-Slab&45684-35-3-4-x-79&71573-Radiance&45464-Russet&45138-1-4-Top-Lite
https://www.pella.com/ideas/doors/entry-doors/front-door-design-ideas/

- Interior
https://www.forbes.com/home-improvement/painting/interior-house-painting-cost/

- Fridge
https://witanddelight.com/2022/03/fridge-organization-system/

- Oven
https://www.lg.com/us/cooking-appliances/lg-ltgl6937f-double-oven-slide-in-gas-range

- Window
https://www.thompsoncreek.com/blog/why-are-window-panes-made-of-glass/
https://www.guttermanservices.com/questions-to-ask-when-buying-replacement-windows-for-my-ashburn-va-home/
https://www.photowall.com/us/window-bay-view-canvas-print
https://www.architecturaldigest.com/reviews/windows/types-of-windows

- Corner
https://thegraphicsfairy.com/free-embroidery-corner-design/
https://stock.adobe.com/contributor/202642025/baoyan?load_type=author&prev_url=detail
https://www.vecteezy.com/vector-art/954079-brown-vintage-frame-border-and-corner-set

- Unit
https://www.forbes.com/home-improvement/living/types-of-house-styles/

**Project Idea Description (Pre-MVD):**

  For my project, my goal is to create an adjustable model of a three-dimensional environment. The focus will be on housing units. I want to take a blueprint and create a model when scanned by a mobile application. This model will be able to be changed if parts were to be moved. Example is if we have a fridge target on top of the main house layout and move the fridge from one side of a room to the other, it should be reflected when scanned again. Adding in the feature to click a button to add and remove the roof would give a better idea of the outside of the house. Being able to scale the house to get a different view would also be beneficial. Potentially, I want there to also be a way to change certain features of this house model like paint color, utilities, etc. This would be in a separate menu accessed from the application.
  
  For AR technology, I want to use Vuforia 10.21.3 in Unity. I am the most comfortable with Vuforia and I believe should be able to have most of the work done with this as the project is mostly getting models/targets to stick to each other to create a larger model. I would like to try to incorporate different technologies and APIs to see what works better. Something that I might look into working with is dynamic visualization techniques to show depth inside of the model, similarly to a terrain map. I might try to look into using OpenCV in Unity in tandem with Vuforia to help with tracking and computer vision. I would like to incorporate a SLAM library and see if this would improve tracking and environment mapping for this project. I am thinking of trying to see if I can use Vuforia for object recognition and possibly Google’s ARCore for mapping and creating a more accurate localization of the environment. There may be more AR technology and APIs that I may end up using, but for now I want to focus on Vuforia and SLAM with the possibility of others. 
  
  An area of AR that I want to focus on with this project is AR tracking. I have not worked with SLAM, but I would like to see how that would work with something like Vuforia to help recognize the location of the targets themselves. This project focuses on tracking multiple targets on a large map and creating a tree-dimensional model based on this map. I have never tried to track multiple objects at once and create different outcomes based on what variation there is, so hopefully this is what I can achieve. Originally, I was planning on tracking small targets using graph paper and adding a wall to each. This can be overbearing on the actual program, as having too many targets can lead to much more errors. Eventually, I realize that I can just track certain parts of the house. I plan on using targets to track the corners of the housing unit. This way, I only need to track where walls should connect between if the corner targets line up with one another. Otherwise, an error would be output. Other parts that I can program in is a target for doors and windows to add to the walls. The interior walls would also be seperate targets, as they behave slightly differently. Furniture would also have it's own individual targets. There will be lots of targets in this project, so trying to have enough to make a house while keeping them low is a key factor. I predict that a basic house would require 16 targets or so at most for 4 corners, doors and windows, furniture, and interior walls. Overall, my goal is to create an three-dimensional model of a housing unit that can be adjusted in multiple ways according to what a blueprint shows.
