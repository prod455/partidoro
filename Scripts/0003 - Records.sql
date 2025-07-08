USE Pomodoro;

IF OBJECT_ID('Records') IS NULL
BEGIN
	CREATE TABLE Records (
		Id INT IDENTITY(1,1) NOT NULL,
		RecordDate DATETIME DEFAULT GETDATE(),
		ElapsedTime TIME NOT NULL,
		TimerMode VARCHAR(12) NOT NULL,
		IntervalCount TINYINT DEFAULT 0,
		TaskId INT,
		ProjectId INT,
		CONSTRAINT PK_Records_Id
			PRIMARY KEY (Id),
		CONSTRAINT FK_Records_Tasks FOREIGN KEY (TaskId)
			REFERENCES Tasks(Id),
		CONSTRAINT FK_Records_Projects FOREIGN KEY (ProjectId)
			REFERENCES Projects(Id)
	);
END
