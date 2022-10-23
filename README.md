# Matchmaking API

![image](https://user-images.githubusercontent.com/17746816/197415960-9c9f6270-f600-46d0-8f32-46514ebd9b48.png)



# How it works

### Step by step

#### Matchmaking

1 Server adds players to the matchmaker
2 The matchmaker matches and adds players together to sessions
3 When a session has reached the minimum requirements to start a game, it gets added to a "ready sessions" collection
4 Server fetches the ready sessions, containg a match id and a collection of players
