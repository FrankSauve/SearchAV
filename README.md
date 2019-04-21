# Radio Canada - Search Engine for Audio and Video Media
[![Build Status](https://dev.azure.com/francoiscrisposauve/SearchAV/_apis/build/status/SearchAV-ASP.NET%20Core-CI)](https://dev.azure.com/francoiscrisposauve/SearchAV/_build/latest?definitionId=2)

[Project Description](https://docs.google.com/document/d/1BW1_ryxb9ybWWSz8yv0q_4uwVtmrzDudvvWG6gARVAk/edit)

**Name and email address of contact:** Xavier K. Richard, Digital innovation coordinator. xavier.kronstrom.richard@radio-canada.ca

**Elevator pitch description at a high level:** Create a tool that will help journalists and editors working at Radio-Canada. The tool will be able to search through videos and audio files in a database, providing results with timestamps where a searched quote appeared.

## Team members 

Concordia Students that are working on the project:

Team members          | Email                         | Student Id   |Github Usernames
------------          | --------------------          | ------------ |----------------
Sarbeng Frimpong      | sarbeng4@hotmail.com          | 29344039     | Sarb-F
Raphaelle Giraud      | Raph1105@hotmail.com          | 27514204     | Raph1105
William Leclerc       | w.dfj.leclerc@gmail.com       | 27424973     | LeCleric
Francois Crispo-Sauve | francoiscrisposauve@gmail.com | 27454139     | franksauve
William Kingbede      | williamkingbede@gmail.com     | 27324383     | williamkingbede
Ayoube Akaouch        | ayoube.akaouch@gmail.com      | 27755341     | ayoubeakaouch
Philippe Kuret        | philippekuret@gmail.com       | 27392680     | philippekuret
~~Anania Yeghikian~~      | ~~anania.y@hotmail.com~~         | ~~27484526~~     | ~~anania-y~~


This project is currently being prepared as part of our course, SOEN 490: Capstone Software Engineering Design Project. 

## Technology stack
Purpose            | Technology
------------       | -------------------- 
Client side        | React with TypeScript
Styling            | Bulma
Server side        | ASP.NET Core (C#)
Testing            | Xunit
Continuous integration | Azure Pipelines
Containerization   | Docker

## Installing the required packages
Install npm packages
```
cd RC-SpeechToText
npm install
```
Install nuget packages
```
nuget restore
```

## Running the solution in Visual Studio
Open the RC-SpeechToText.sln file in Visual Studio
Run the solution with IIS Express.
The server will run on localhost at the port designated in launchSettings.json

## Using the application
- You must sign in with a google account
- You can upload new mp3 or mp4 files by clicking "Parcourir" or drag and dropping a file in the upload box.
- Once the transcription is done, you can click on the file in the dashboard to view the video player and edit the transcription.
