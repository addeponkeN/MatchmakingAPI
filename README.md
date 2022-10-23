# Matchmaking API

![image](https://user-images.githubusercontent.com/17746816/197415960-9c9f6270-f600-46d0-8f32-46514ebd9b48.png)



# How it works

## Step by step

### Matchmaking

- Server adds players to the matchmaker
- The matchmaker matches and adds players together to sessions
- When a session has reached the minimum requirements to start a game, it gets added to a "ready sessions" collection
- Server fetches the ready sessions, containg a match id and a collection of players
