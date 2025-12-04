# Directory Browser API + Frontend
### Written by Connor McDermott

This project is written on Linux and expects to use Linux filepaths.
I used .NET 8 Core, nvim, dotnet cli, with a TS frontend, no UI libraries or frameworks, just tsc.
Testing was done with postman and firefox.

## Running the project

1. Clone the repository.
1. Navigate to the project directory.
1. Run `npm install && tsc` to install tsc and transpile the typescript.
1. Run `dotnet run` to start the backend server.

## Overview

The primary endpoint is at /api/path/<path>, allowing for easy browsing of directories.
The frontend also allows for simple browsing of directories and files, where the URL path is localhost:3000/app<path>.

The frontend SPA works by hijacking navigate() and then sending the App root div to a renderer function.
I would've liked to separate out folderView into more components and spend more time organizing, but that felt out of scope.

