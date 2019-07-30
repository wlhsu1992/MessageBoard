CREATE PROCEDURE dbo.usp_Message_Add
(
	@Title NVARCHAR(20),
	@Content NVARCHAR(500),
	@MemberID INT
)
AS
BEGIN
	INSERT INTO Message(Title,Content,MemberID) 
	VALUES(@Title,@Content,@MemberID)
END
GO

CREATE PROCEDURE dbo.usp_Message_List
AS
BEGIN
	SELECT Member.Name,Member.Account,Message.ID,Message.Title,Message.CreateTime 
	FROM Message Join Member ON Message.MemberID = Member.ID 
	ORDER BY ID DESC
END
GO

CREATE PROCEDURE dbo.usp_Message_GetMessage
(
	@MessageID INT
)
AS
BEGIN
	SELECT Member.Name,Member.Account,
	Message.ID,Message.Title,Message.Content,Message.CreateTime,Message.MemberID
	FROM Message Join Member ON Message.MemberID = Member.ID
	WHERE Message.ID = @MessageID
END
GO

CREATE PROCEDURE dbo.usp_Message_Get
(
	@MessageID INT
)
AS
BEGIN
	SELECT ID,Title,Content,MemberID FROM Message
	WHERE ID = @MessageID
END
GO

CREATE PROCEDURE dbo.usp_Message_Search
(
	@keyword NVARCHAR(20)
)
AS
BEGIN
	SELECT Member.Name,Member.Account,Message.ID,Message.Title,Message.CreateTime
	FROM Message Join Member ON Message.MemberID = Member.ID
	WHERE Message.Title LIKE @keyword  
	ORDER BY ID DESC
END
GO

CREATE PROCEDURE dbo.usp_Message_Update
(
	@NewTitle NVARCHAR(20),
	@NewContent NVARCHAR(500),
	@MessageID INT,
	@MemberID INT
)
AS
BEGIN
	UPDATE Message
	SET Title = @NewTitle,Content = @NewContent
	WHERE ID = @MessageID AND MemberID = @MemberID
END
GO

CREATE PROCEDURE dbo.usp_Message_Delete
(
	@MessageID INT
)
AS
BEGIN
	DELETE Message
	FROM Message
	WHERE Message.ID = @MessageID;
	DELETE ReplyMessage
	FROM ReplyMessage
	WHERE ReplyMessage.MessageID = @MessageID
END
GO
