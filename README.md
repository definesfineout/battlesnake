# Snakes who play [Battlesnake](https://play.battlesnake.com/) 🐍

Developed live on stream at [twitch.tv/defiines](https://twitch.tv/defiines)

- **SillyBoi** implements naive approaches to survival and finding food
- **Skagit** plans to implement some sort of hamiltonian cycle algorithm before augmenting with flood-fill
- **Sifl** is controlled by human input via gamepad, more sock puppet than snake
- **Olly** will also be gamepad-controlled, but will try to help avoid walls and other snakes

# Acknowledgments

- I started with the [Battlesnake C# Starter](https://github.com/neistow/battlesnake-starter-csharp) by [neistow](https://github.com/neistow)
- Some code rapidly (sloppily) ported from Python project [snake](https://github.com/chuyangliu/snake) by [chuyangliu](https://github.com/chuyangliu)
- Hamilton solver code ported from [Hamiltonian Cycle on Tutorials Point](https://www.tutorialspoint.com/Hamiltonian-Cycle) by Sharon Christine

# Resources
## Classic Algorithmic Approaches
### Hamiltonian Cycle
- [Snake: Hamiltonian Cycle - Literature review](https://kychin.netlify.app/snake-blog/hamiltonian-cycle/)
    - Based largely upon [Efficient solution for finding Hamilton cycles in undirected graphs](https://springerplus.springeropen.com/articles/10.1186/s40064-016-2746-8)
    - Also [Nokie 6110 Part 3: Algorithms by John Tapsell](https://johnflux.com/2015/05/02/nokia-6110-part-3-algorithms/)
        - itself, based upon [Prim's algorithm (Maze Generation)](https://en.wikipedia.org/wiki/Maze_generation_algorithm#Randomized_Prim's_algorithm)

### Flood Fill

## Deep ML Approaches
[Training a Snake Game AI: A Literature Review](https://towardsdatascience.com/training-a-snake-game-ai-a-literature-review-1cdddcd1862f)
### Reinforcement Learning
- [How to Teach an AI to Play Games w/ Deep Reinforcement Learning](https://towardsdatascience.com/how-to-teach-an-ai-to-play-games-deep-reinforcement-learning-28f9b920440a)
- [Deep learning vs. machine learning in Azure Machine Learning](https://docs.microsoft.com/en-us/azure/machine-learning/concept-deep-learning-vs-machine-learning) - discussions on types of neural networks
- [Reinforcement learning (preview) with Azure Machine Learning](https://docs.microsoft.com/en-us/azure/machine-learning/how-to-use-reinforcement-learning)
    - [How to use Azure ML Reinforcement Learning](https://github.com/Azure/MachineLearningNotebooks/blob/master/how-to-use-azureml/reinforcement-learning/README.md)


## Human Input

- https://stackoverflow.com/q/3929764/120990
- https://stackoverflow.com/a/12988935/120990
  - [C++ Adapter](https://pastebin.com/AiecKJjZ)
- Alternative https://opentk.net/