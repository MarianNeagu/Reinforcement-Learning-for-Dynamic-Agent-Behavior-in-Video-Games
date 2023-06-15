# PAS Project - RL Agents for enemys in video games

## Setup
- steps to be added

## Start the training
- in **\venv\Scripts\** run **.\activate**
- to start a new training session with the default config file: 
```console
mlagents-learn --run-id=IDNAME
```
- to resume a training:
```console
mlagents-learn --run-id=IDNAME --resume
```
- to override a training:
```console
mlagents-learn --run-id=IDNAME --force
```
- to train a model with a custom config file add its path:
```console
mlagents-learn config/config.yaml --run-id=IDNAME 
```
