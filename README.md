# Matchmaking API

![image](https://user-images.githubusercontent.com/17746816/197415960-9c9f6270-f600-46d0-8f32-46514ebd9b48.png)



## Features

- Supports multiple games
- Supports millions of players matchmaking at the same time
- Match players together by latency (player location)
- Supports Drop-in matchmaking (Players can join an ongoing session)


## How it works

### Step by step

#### Matchmaking

- Client adds players to the matchmaker
- The matchmaker matches and adds players together to sessions
- When a session has reached the minimum requirements to start a game, it gets added to a "ready sessions" collection
- The client can now fetch the "ready sessions" collection, containing collections of match ids and players
