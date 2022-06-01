﻿Written by Aidan Lethaby, September 2019

Note to graders:
	The features I added were a search function and one instance of multithreading. The search function can display a list 
	and give a count of all the cells that contain a certain value you specify. I also tried my hand at using background 
	workers and added one to allow the user to drag, close, open a new spreadsheet, or access help while cells are being 
	recalculated due to a contents change.
	
Using:
	DependencyGraph.dll		6 September 2019 Version (original version turned into gradescope)
	Formula.dll				13 September 2019 Version (original version turned into gradescope)
	Spreadsheet.dll			4 October 2019 Version (updated exception messages, exception handling)
	SpreadsheetPanel.dll	27 September 2019 Version (original version provided in PS6Skeleton)

29 September 2019
Initial design thoughts about the project:
	- I don't think any buttons should be utilized in the UI for cleanliness and also to mimic excel more closely.
	- Must figure out a way to set focus initially to cell A1: using Shown event.
	- Must figure out how to have popups: using Messageboxes.
	- Update lookup in PS4/PS5 to give a useful message when throwing an argument exception.
	- So far my GetSelectedCell and SetSelectiveCell methods do most of the work. Is there code I can simplify.
	- I think changing the arrow keys to select a cell might be a cool addition.
2 October 2019
Further thoughts:
	- OpenFileDialog and SaveFileDialog will be useful, use them when implementing save and open.
	- Looks like circular exceptions are not handled properly in all cases in PS5. I'll have to go edit that.
4 October 2019
Further thoughts:
	- What feature should I add?
	- Searching for values. Maybe some backgound workers.
	- Is this the optimal way to set up a background worker? Seems like a lot of work.
	- I should also display the number of cells that contain the value. Good info to know and fairly easy to add.