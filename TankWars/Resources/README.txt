Created by Aidan Lethaby and Sean Richens for CS3500

Server:


Implementation Details:
3 December 2019
- Made the tank ID the socket states ID to help when updating the right tank in the model when we recieve a command
- Made it so we only process the most recent control command in the off chance we recieve multiple per frame
- We chose to restrict world size and and milliseconds per frame both greater than 0
- We chose to restrict frames per shot to be a non-negative number
4 December 2019
- Tanks are represented by 8 points when checking for collision with walls.  This is because when only checking the corners, the tanks could straddle walls
5 December 2019
- Updates database and closes web server when user exits the game server
- The powerup delay doesn't count down when the maximum number of powerups is on the screen.  This is to prevent a powerup from immediately spawning when one is picked up.
- If there are multiple people with the same name in one game, only one of them is recorded in the database for the game (the rest are ignored)
- The maximum number of powerups was added as an option to the settings file, and that number can not be less than 0.

Client:

Unique Features:
- Custom Tank Explosions
- Rainbow beams and powerups
	- We created a method that allowed us to dynamically change the color of powerups and beams to create the rainbow effect
- You can hold down the main fire button, fire a beam, and keep firing the main projectile without letting go of the main fire button

Implementation Details:
15 November 2019
- Create a class representing each drawable object and the world
- Create a constant class to hold relevant constant values
- Utilize a modified LAb11 DrawingPanel to hepl draw the world
- We're representing the control commands as an object, which is serialized and sent once per frame.  We think this is a simple way to go about it.
- Use an interface to keep track of objects with IDs in a generic helper method
16 November 2019
- After some deliberation we agreed we should focus on the view first, we belive it will be the best starting point as it is an easy way to visualize success.
18 November 2019
- We made the window size dynamic, based off the size of the user's screen
21 November 2019
- Powerups and power beams are rainbow colored!  
- In order to make this happen, we assigned beams and powerups ages, describing how many frames they have existed.  We decided to store the ages of 
	powerups in the World, as storing them in the Powerup object would have a bit of a conflict with the way we're updating objects sent from the server
- We ran into the issue of trying to edit the contents of dictionaries while iterating through them.  
	Now, instead of iterating through the dictionary itself, we iterate through an array that has a copy of the dictionary's keys.
22 November 2019
- The Form listens to keyboard presses and the DrawingPanel listens to the mouse