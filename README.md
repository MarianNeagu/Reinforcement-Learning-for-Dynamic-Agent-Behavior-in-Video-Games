# Reinforcement Learning for Dynamic Agent Behavior in Video Games


https://github.com/MarianNeagu/Reinforcement-Learning-for-Dynamic-Agent-Behavior-in-Video-Games/assets/71409464/5cd6292d-1c1e-4611-82b7-66da561d6dce


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

<img src="https://github-production-user-asset-6210df.s3.amazonaws.com/71409464/251377241-84a420f4-e4a8-42e4-9bec-2e0a557f043b.png" width=100%>



