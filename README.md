# Technical Project - Blackjack WebApp

This is a Blackjack web app with a .NET 10 API and a React + TypeScript frontend. Game rules follow
[Bicycle Cards](https://bicyclecards.com/how-to-play/blackjack).

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 24](https://nodejs.org)

## Build
```bash
dotnet build Blackjack.slnx
cd frontend
npm install
npm run build
```

## Test
```bash
dotnet test Blackjack.slnx
cd frontend && npm test
```

## Run in Rider
1. Open `Blackjack.slnx`.
2. Run `npm ci` in the `frontend` folder (first time only).
3. Select the **Fullstack** run configuration and run it (you may need to select your Node v24 installation from the Rider menu).

The API starts on http://localhost:5080. The browser opens http://localhost:5173 once the API is ready.

## Run from the command line
From the repo root, in two terminals:

```bash
dotnet run --project backend/src/Blackjack.Api
```

```bash
cd frontend
npm run dev
```