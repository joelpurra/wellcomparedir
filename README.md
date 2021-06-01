# [WellCompareDir](https://joelpurra.com/projects/WellCompareDir/)

<p class="donate">
  <a href="https://joelpurra.com/donate/proceed/?amount=5&currency=usd"><kbd>Donate $5 now</kbd></a>
  <a href="https://joelpurra.com/donate/proceed/?amount=25&currency=usd"><kbd>Donate $25 now</kbd></a>
  <a href="https://joelpurra.com/donate/proceed/?amount=100&currency=usd&invoice=true"><kbd>Donate $100 now</kbd></a>
  <a href="https://joelpurra.com/donate/"><kbd>More options</kbd></a>
</p>

*A tool to compare and select images with the same name from two folders, and copy to a third folder*



> ## ⚠️ This project has been archived
>
> No future updates are planned. Feel free to continue using it, but expect no support.



[![Screenshot of the main window of WellCompareDir from 2010-11-22](./WellCompareDir.Web/screenshot/2010-11-22/wellcomparedir_main_window_2010-11-22_1528_01.jpg)](https://joelpurra.com/projects/WellCompareDir/)  
*Screenshot of the main window, showing a "bad image" to the left and a "good image" to the right. The right file is preferred and will be copied to the output directory. Either hit <kbd>space</kbd> to get the automatically chosen image, <kbd>←</kbd> or <kbd>→</kbd> or click to select manually*



## Download and installation

[Download and run installation package.](https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir.Installer.msi) A shortcut will be installed, find it from your start menu to run the program.


- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir.Installer.msi">Installation package</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir_2010-11-22_1528_release.7z">Release version (no installation)</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir_2010-11-22_1528_debug.7z">Debug version (no installation)</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir_2010-11-22_1528_source.7z">Source code</a>
- <a href="https://joelpurra.github.io/wellcomparedir/">Latest source code on github</a>



## Manual

Use this tool to merge images from two folders to a third, output folder. It is important that the images have the same name if they are considered the same image. Differences in file extensions are ignored.



### Usage examples

- Compare images before and after batch modifications of contrast, color balance, white balance and such.
- Compare images received in several batches from a third party.
- Compare photos copied several times from an intermittently faulty digital camera memory card.



### Get started

1. Click the left folder button to browse to the left folder for comparison.
1. Click the right folder button to browse to the right folder for comparison.
1. Click the bottom right folder button to browse to the output folder where images will be copied to.
1. Select images to compare in the lists at the top.
1. Choose whether to use the left or the right image, by clicking. The selected image will be copied to the output directory.
1. The next image will be displayed. Repeat.



### Tips

- In the list of images from the two selected folders, both left and right side images appear in comined alphabetical order with gaps where there is no match on the other side.
- Unique images are greyed out and images with the same name are not.
- If two images look similar, have a look at the image dimensions and file size. The largest image in pixels, or the largest file in bytes, is noted with a small green arrow.
- Supports images in the following formats
  - JPEG (.jpg, .jpeg, .jpe)
  - TIFF (.tif, .tiff)
  - PNG (.png)
  - GIF (.gif)
  - Windows Bitmap (.bmp)
  - Windows Media Photo (.hdp, .jxr, .wdp)
  - Windows Icon (.ico)



### Shortcut keys

Speed up your work with shortcut keys, the way WellCompareDir was meant to be used.


**Primary shortcut keys**


- <kbd>Space</kbd> **Space** Use the automatically suggested image and advance to the next matching image pair.
- <kbd>←</kbd> **Left arrow key** Use the left image and advance to the next matching image pair.
- <kbd>→</kbd> **Right arrow key** Use the right image and advance to the next matching image pair.
- <kbd>↑</kbd> **Down arrow key** Advance to the next matching image pair.
- <kbd>↓</kbd> **Up arrow key** Back to the previous matching image pair.



**Secondary shortcut keys**

- <kbd>Ctrl</kbd>+<kbd>←</kbd> **Control + Left arrow key** Use the left image.
- <kbd>Ctrl</kbd>+<kbd>→</kbd> **Control + Right arrow key** Use the right image.
- <kbd>Ctrl</kbd>+<kbd>↑</kbd> **Control + Down arrow key** Advance to the next image.
- <kbd>Ctrl</kbd>+<kbd>↓</kbd> **Control + Up arrow key** Back to previous image.



### Requirements

Normal Windows computers that have installed Microsoft's monthly patches should be able to use WellCompareDir with no extra hassle.

- Microsoft Windows
- Microsoft Windows Installer, if downloading the installer package
- <a href="https://www.microsoft.com/net/">Microsoft .NET Framework 4.0</a>
- <a href="https://www.7-zip.org/">7-Zip</a> if downloading a non-installer package
- For non-Windows users: <a href="https://www.mono-project.com/">Mono</a> has not been tested, please let me know if you have any success.



## History

A brief summary of the changes from version to version.



**Latest version:** *unreleased*

- Allow comparing images from multiple folders.
- Skip non-matching files by default
- More shortcut keys
- Fixed a few possible crashes
- Cleaned up code
- Moved the [latest source code to github](https://joelpurra.github.io/wellcomparedir/).

Please compile this version from [the WellCompareDir source on github](https://joelpurra.github.io/wellcomparedir/).



**2010-11-22 15:28**

- Added installer for simpler distribution
- Match images with different extensions
- Show only supported image format extensions
- Make images buttons
- Simplify shortcut keys
- Added copyright information


- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir.Installer.msi">Installation package</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir_2010-11-22_1528_release.7z">Release version (no installation)</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir_2010-11-22_1528_debug.7z">Debug version (no installation)</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-22_1528/WellCompareDir_2010-11-22_1528_source.7z">Source code</a>


**2010-11-19 02:39**

First public release

- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-19_0239/WellCompareDir_2010-11-19_0239_release.7z">Release version</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-19_0239/WellCompareDir_2010-11-19_0239_debug.7z">Debug version</a>
- <a href="https://files.joelpurra.com/projects/wellcomparedir/2010-11-19_0239/WellCompareDir_2010-11-19_0239_source.7z">Source code</a>



## About

Originally developed in November 2010, WellCompareDir was created to sort <a href="https://www.froer.nu/search/random/">thousands of product images for Fröer.nu</a> in from several folders. These product images are delivered by the product suppliers in different formats and qualities each year. Replacing one image with the most recent version did not always mean an improvement, and simply relying on file size or image dimensions did not help all of the time. Manual selection was the answer, and a specific tool to achieve this seemed like the most effective way.



---



[WellCompareDir](https://joelpurra.com/projects/WellCompareDir/) Copyright &copy; 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017 [Joel Purra](https://joelpurra.com/). All rights reserved. Released as free software under the [GNU General Public License](https://www.gnu.org/licenses/gpl.html). [Your donations are appreciated!](https://joelpurra.com/donate/)
