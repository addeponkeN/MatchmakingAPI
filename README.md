# Matchmaking API

![image](https://user-images.githubusercontent.com/17746816/199473072-dbff4558-a468-49ca-a09c-38ceb4bcb9e4.png)




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
- The client can now fetch the "ready sessions" collection, containing collections of session ids and players

.
.

## Early Prototype Programmer Art Diagram :D

![early prototype diagram](https://user-images.githubusercontent.com/17746816/197418742-647a11e6-1ca4-4763-993f-ae68476cb3b9.png)
