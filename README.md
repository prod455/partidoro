# Partidoro
Achieves more traceability by partitioning pomodoros into tasks and projects. 
## Application
Core functionalities.
### Timer
There are three available timer modes: Normal, Short Break and Long Break.

When a Normal pomodoro is completed, the timer switches to a Short Break pomodoro and after four consecutive Normal pomodoros, it switches to a Long Break pomodoro.

The timer works by subtracting one second from its current pomodoro's total duration, switching to the next type when the time expires.

The timer start command can attach a project and task to the created timer record and also update the timer record by its id.

The timer start command does have a flag for showing a Windows notification when the timer mode switches. 
### Record
A pomodoro timer record is created after successfully exiting the timer command.

The record command branch can list and update records.

The record list command can list records by project and task id.
### Task
The task command branch can add, list and update tasks.

The task list command can list tasks by project and task id.

The task add command requires a title argument.

The task update command requires a id argument.

Both task add and update commands can attach a created task to an existing project.
### Project
The project command branch can add, list and update projects.

The project list can list projects by project and task id.
## Domain
List of models used.
### Record
Pomodoro timer record.
#### Properties
- Id: generated unique identifier.
- RecordDate: pomodoro's start date and time.
- ElapsedTime: pomodoro's elapsed time.
- TimerMode: pomodoro's timer mode
### Task
Describes pomodoros and estimates a quantity of pomodoros per task.
#### Properties
- Id: generated unique identifier.
- Title: task's title, limited by 50 characters.
- ActualQuantity: actual quantity of done pomodoros per estimated quantity.
- EstimatedQuantity: estimated quantity of done pomodoros.
- Note: task's description, limited by 150 characters.
### Project
Group of tasks, separates tasks concerns.
#### Properties
- Id: generated unique identifier.
- Name: project's name, limited by 50 characters.
- Description: project's description, limited by 150 characters.
