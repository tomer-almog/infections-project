# Infections Project

This repository contains 2 projects: the Infections API server, and a Unit tests project.

## Installation

Clone the repo locally:

```git clone https://github.com/tomer-almog/infections-project.git```

Navigate to the project folder:

```cd infections-project/InfectionsProject```

Make sure you have EF Core Tools installed, and restart your terminal after:

```dotnet tool install --global dotnet-ef```

Execute DB migrations:

```dotnet ef database update```

## Usage

In your IDE, run the InfectionsProject. Make sure it is running on localhost:5000.
Then, execute the tests in Tests.cs file inside UnitTests project.

Thank you!
