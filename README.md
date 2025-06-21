# Partidoro
Achieves more traceability by partitioning pomodoros into tasks and projects 
## Application
Core functionalities.
### Timer
There are three available timer modes: Normal, Short Break and Long Break

When a Normal pomodoro is completed, the timer switches to a Short Break pomodoro and after four consecutive Normal pomodoros, it switches to a Long Break pomodoro.

The timer works by subtracting one second from its current pomodoro's total duration, switching to the next type when the time expires.
## Domain
List of models used.
### Record
Pomodoro timer record.
#### Properties
- Id: generated unique identifier.
- RecordDate: pomodoro's start date and time.
- ElapsedTime: pomoro's elapsed time.
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
