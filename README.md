# AIBracket

## Project Architecture

![Project Architecture](https://github.com/sknnywhiteman/AIBracket/blob/master/Images/Architecture.png "Project Architecture")

## Game Ideas:  
 *Pacman
   * Can Program Ghost or Pacman


## Architecture:
  * Central Database (sqlite) 
  * Data project (Interfacing with database)
  * Game project (handles all games and logic)
  * Web project (displays live games, data viewing)
  * API project (interfaces clients with game project)
  * Everything written in C#, web written in angular

## Web project:
  * Angular 7
  * Can select between different games (pacman, etc.)
  * Displays live matches
  * Displays leaderboards
  * Displays statistics of AI
  * Search players/AIs
  * Players sign up to receive a private key
  * dotnet core serverside which communicates with common logic

## API project:
  * Validation
  * Provides a persistent client state
  * TCP socket for client communications

## Common Logic:
  * Computes games
  * Interfaces with web project to display games to users
  * Interfaces with API project to communicate actions and gamestate to users
  * Interfaces with data project to save data and log
  * Stores live game data in memory for web project, when finished store in DB 

## Data project:
  * Handles user authentication (for web)
  * Generates private keys for AI
  * Interfaces directly with database
  * Contains controllers for accessing individual tables

## Database:
 * Users
    * Username
    * Password
    * Password Salt
    * AI IDs
    * Created Date
 * AI
    * Name
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
