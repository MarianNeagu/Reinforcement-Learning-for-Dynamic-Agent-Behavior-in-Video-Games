# Reinforcement Learning for Dynamic Agent Behavior in Video Games

## Setup
- Follow the steps [from the official guide](https://github.com/Unity-Technologies/ml-agents/blob/release_20_docs/docs/Installation.md).

## Start the training
- In **\venv\Scripts\\** run **.\activate**
- To start a new training session with the default config file: 
```console
mlagents-learn --run-id=IDNAME
```
- To resume a training:
```console
mlagents-learn --run-id=IDNAME --resume
```
- To override a training:
```console
mlagents-learn --run-id=IDNAME --force
```
- To train a model with a custom config file add its path:
```console
mlagents-learn config/config.yaml --run-id=IDNAME 
```
- To open the TensorBoard:
```console
tensorboard --logdir results
```
