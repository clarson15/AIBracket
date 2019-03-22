# AIBracket

This is a live document. Anything on this page is subject to change, and should be kept up to date with the current version of the application.

## Project Architecture

![Project Architecture](https://github.com/sknnywhiteman/AIBracket/blob/master/Images/Architecture.png "Project Architecture")

## Game Ideas:  
 * Pacman
   * Can Program Ghost or Pacman


## Architecture:
  * Database (SQLite) 
  * Data project (Interfacing with database, DLL)
  * Common Logic project (handles all games and logic, DLL)
  * Web project (displays live games, data viewing)
  * API project (interfaces clients with game project)
  * Everything written in C#, web written in angular

## Web project:
  * Angular 7
  * Handles authentication
  * Can select between different games (pacman, etc.)
  * Displays live matches
  * Displays leaderboards
  * Displays statistics of AI
  * Search players/AIs
  * Players sign up to receive a private key
  * dotnet core serverside which communicates with API through socket

## API project:
  * Validation
  * Provides a persistent client state
  * TCP socket for client communications
  * Interfaces with data project to save data and log
  * Stores live game data in memory, when finished store in DB 

## Common Logic:
  * Computes games
  * Stores game logic and models

## Database:
 * Users
    * Username
    * UID
    * Password
	* First Name
	* Last Name
	* Email
	* Phone Number
 * AI
    * Name
    * ID
    * UID
    * Private Key
    * Associated Game
    * Created date
    * Last connected
 * Games (one table for each game, this is pacman example)
    * Player IDs
    * Game History??
    * Outcome (score of pacman)
    * Time elapsed
    * Time created

## Setup
1. Download [dotnet core 2.2](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.103-windows-x64-installer)
2. Download [NodeJS](https://nodejs.org/en/)
3. Run `dotnet ef database update` from console in AIBracket.Web directory after building to seed database
