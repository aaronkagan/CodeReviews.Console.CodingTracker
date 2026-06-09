# Coding Tracker


## Thought process
- To create the application in a modular way so that extending the application will be easier in the future
- What is the user creating? (Coding Session)
- What information belongs to the coding session?
- What can the user do with the coding session? (CRUD)
- 

### Process
- What are the menu options? (ie what can the user do?)
- What data must be stored in the DB?
- What are the main classes?

## What I found easy

## What I found hard

- Identifying the classes
- Architecture

## Requirements

- This application has the same requirements as the habit tracker, except that now you'll be logging your daily coding time
- To show the data on the console, you should use the Spectre.Console library
- Users need to be able to input the date of the occurrence of the coding session
  - You should tell the user the specific format you want the date and time to be logged and not allow any other format
- The user should be able to input the start and end times manually
  - The user shouldn't input the duration of the session. It should be calculated based on the Start and End times
- The application should store and retrieve data from a real database
  - When the application starts, it should create a sqlite database, if one isn’t present
  - It should also create a table in the database, where the session will be logged
- The users should be able to insert, delete, update and view their logged habit
- You should handle all possible errors so that the application never crashes
- Follow the DRY Principle, and avoid code repetition
- You need to use Dapper ORM for the data access instead of ADO.NET

## Challenges
- Add the possibility of tracking the coding time via a stopwatch so the user can track the session as it happens
- Let the users filter their coding records per period (weeks, days, years) and/or order ascending or descending
- Write unit tests for a few methods in the project. Any method that outputs data and doesn't connect to a database can be unit tested. A good example is any method that deals with validation and testing your data-retrieving methods with different filters

