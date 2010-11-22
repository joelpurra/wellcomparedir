<%@ Page Language="C#" ResponseEncoding="UTF-8" %>

<!DOCTYPE html>
<html class="no-js" lang="en-us">
<head>
	<meta charset="utf-8" />
	<title>joelpurra.se/Projects/WellCompareDir by Joel Purra</title>
	<meta name="description" content="WellCompareDir is a tool to compare and select images with the same name from two folders, and copy to a third folder." />
	<link rel="stylesheet" href="css/blueprint/screen.css" type="text/css" media="screen, projection" />
	<link rel="stylesheet" href="css/blueprint/print.css" type="text/css" media="print" />
	<!--[if lt IE 8]><link rel="stylesheet" href="css/blueprint/ie.css" type="text/css" media="screen, projection" /><![endif]-->
	<link rel="stylesheet" href="css/blueprint/plugins/buttons/screen.css" type="text/css"
		media="screen, projection" />
	<link rel="stylesheet" href="css/screen.css" type="text/css" media="screen, projection" />
	<!--[if lt IE 9]><script type="text/javascript" src="javascript/modernizr-1.6.min.js"></script><![endif]-->
	<link href="http://fonts.googleapis.com/css?family=Neucha:regular" rel="stylesheet"
		type="text/css">
</head>
<body>
	<div class="container prepend-top">

		<header>
			<p>
				<em class="logo">WellCompareDir</em><span class="tagline"><span class="not-necessary">
					is </span>a tool to compare and select images with the same name from two folders,
					and copy to a third folder<span class="not-necessary">.</span></span>
			</p>
		</header>

		<nav>
			<ol>
				<li><a href="#download">Download</a></li>
				<li><a href="#screenshots">Screenshots</a></li>
				<li><a href="#manual">Manual</a></li>
				<li><a href="#history">History</a></li>
				<li><a href="#about">About</a></li>
			</ol>
		</nav>

	</div>

	<hr class="space" />

	<section id="download" class="container">
		<h1 class="hide">
			Download</h1>
		<div class="span-12">
			<p>
				<a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_release.7z" id="download-latest"
					class="button block">Download latest version</a>
			</p>
			<h2>
				Latest version: 2010-11-22 15:28</h2>
			<ul>
				<li>Match images with different extensions</li>
				<li>Show only supported image format extensions</li>
				<li>Make images buttons</li>
				<li>Simplify shortcut keys</li>
				<li>Added copyright information</li>
			</ul>
		</div>

		<aside class="span-12 last">
			<h2>
				Installation</h2>
			<p>
				Unpack into any folder, run WellCompareDir.WPF.exe from there.
			</p>
			<h2>
				Alternatives</h2>
			<ul>
				<li><a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_release.7z">Release
					version</a></li>
				<li><a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_debug.7z">Debug
					version</a></li>
				<li><a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_source.7z">Source
					code</a></li>
			</ul>
		</aside>

	</section>
	<section id="screenshots" class="container">
		<h1>
			Screenshots</h1>
		<p>
			<img src="Screenshot/2010-11-22/wellcomparedir_main_window_2010-11-22_1528_01.jpg"
				alt="Screenshot of the main window of WellCompareDir from 2010-11-22" /><br />
			The main window
		</p>
	</section>
	<section id="manual" class="container">
		<h1>
			Manual</h1>
		<p>
			Use this tool to merge images from two folders to a third, output folder. It is
			important that the images have the same name if they are considered the same image.
			Differences in file extensions are ignored.
		</p>
		<section>
			<h2>
				Usage examples</h2>
			<ul>
				<li>Compare images before and after batch modifications of contrast, color balance,
					white balance and such.</li>
				<li>Compare images received in several batches from a third party.</li>
				<li>Compare photos copied several times from an intermittently faulty digital camera
					memory card.</li>
			</ul>
		</section>
		<section>
			<h2>
				Get started</h2>
			<ol>
				<li>Click the left folder button to browse to the left folder for comparison.</li>
				<li>Click the right folder button to browse to the right folder for comparison.</li>
				<li>Click the bottom right folder button to browse to the output folder where images
					will be copied to.</li>
				<li>Select images to compare in the lists at the top.</li>
				<li>Choose whether to use the left or the right image, using the buttons below the images.
					The selected image will be copied to the output directory.</li>
				<li>Select the next image, repeat.</li>
			</ol>
		</section>
		<section>
			<h2>
				Tips</h2>
			<ul>
				<li>In the list of images from the two selected folders, both left and right side images
					appear in comined alphabetical order with gaps where there is no match on the other
					side.</li>
				<li>Unique images are greyed out and images with the same name are not.</li>
				<li>If two images look similar, have a look at the image dimensions and file size. The
					largest image in pixels, or the largest file in bytes, is noted with a small green
					arrow.</li>
			</ul>
		</section>
		<section>
			<h2>
				Shortcut keys</h2>
			<p>
				Speed up your work with shortcut keys, the way WellCompareDir was meant to be used.
			</p>
			<dl>
				<dt>Left arrow</dt>
				<dd>
					Use the left image and advance to the next image.</dd>
				<dt>Right arrow</dt>
				<dd>
					Use the right image and advance to the next image.</dd>
				<dt>Down arrow</dt>
				<dd>
					Advance to the next image.</dd>
				<dt>Up arrow</dt>
				<dd>
					Back to previous image</dd>
			</dl>
		</section>
	</section>
	<section id="history" class="container">
		<h1>
			History</h1>
		<p>
			A brief summary of the changes from version to version.
		</p>
		<section>
			<h2>
				Latest version: 2010-11-22 15:28</h2>
			<ul>
				<li>Match images with different extensions</li>
				<li>Show only supported image format extensions</li>
				<li>Make images buttons</li>
				<li>Simplify shortcut keys</li>
				<li>Added copyright information</li>
			</ul>
			<ul>
				<li><a href="Screenshot/2010-11-22/wellcomparedir_main_window_2010-11-22_1528_01.jpg">
					Screenshot</a></li>
				<li><a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_release.7z">Release
					version</a></li>
				<li><a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_debug.7z">Debug
					version</a></li>
				<li><a href="download/2010-11-22_1528/WellCompareDir_2010-11-22_1528_source.7z">Source
					code</a></li>
			</ul>
		</section>
		<section>
			<h2>
				2010-11-19 02:39</h2>
			<ul>
				<li>First public release</li>
			</ul>
			<ul>
				<li><a href="Screenshot/2010-11-19/wellcomparedir_main_window_2010-11-19_0239_01.png">
					Screenshot</a></li>
				<li><a href="download/2010-11-19_0239/WellCompareDir_2010-11-19_0239_release.7z">Release
					version</a></li>
				<li><a href="download/2010-11-19_0239/WellCompareDir_2010-11-19_0239_debug.7z">Debug
					version</a></li>
				<li><a href="download/2010-11-19_0239/WellCompareDir_2010-11-19_0239_source.7z">Source
					code</a></li>
			</ul>
		</section>
	</section>
	<section id="about" class="container">
		<h1>
			About</h1>
		<p>
			WellCompareDir was created to sort <a href="http://www.froer.nu/search/random/">thousands
				of product images for Fröer.nu</a> in from several folders. These product images
			are delivered by the product suppliers in different formats and qualities each year.
			Replacing one image with the most recent version did not always mean an improvement,
			and simply relying on file size or image dimensions did not help all of the time.
			Manual selection was the answer, and a specific tool to achieve this seemed like
			the most effective way.
		</p>
		<p>
			WellCompareDir copyright 2010 <a href="http://joelpurra.se/">Joel Purra</a>. Released
			as free software under the <a href="http://www.gnu.org/licenses/gpl.html">GNU General
				Public License</a>.
		</p>
	</section>

	<hr class="space" />

	<footer class="container">
            <a href="http://joelpurra.se/Projects/WellCompareDir/">WellCompareDir</a>. Originally
            coded in November 2010 by <a href="http://joelpurra.se/">Joel Purra</a>.

	</footer>

</body>
</html>
