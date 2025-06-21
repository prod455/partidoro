USE Pomodoro;

IF OBJECT_ID('Records') IS NULL
BEGIN
	CREATE TABLE Records (
		Id INT IDENTITY(1,1) NOT NULL,
		RecordDate DATETIME DEFAULT GETDATE(),
		ElapsedTime TIME NOT NULL,
		TimerMode VARCHAR(12) NOT NULL,
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

IF OBJECT_ID('Records_RecordDate') IS NULL
BEGIN
	CREATE TRIGGER dbo.Records_RecordDate ON dbo.Records 
		AFTER UPDATE
	AS
	BEGIN
	    SET NOCOUNT ON;

		IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

		UPDATE R
		SET RecordDate = GETDATE()
		FROM dbo.Records AS R
		INNER JOIN INSERTED AS I
			ON I.Id = R.Id;
	END
END
