CREATE PROCEDURE dbo.usp_Message_Add
(
	@Title NVARCHAR(20),
	@Content NVARCHAR(500),
	@MemberID INT,
	@PhotoID VARCHAR(20)
)
AS
BEGIN
	INSERT INTO Message(Title,Content,MemberID,PhotoID) 
	VALUES(@Title,@Content,@MemberID,@PhotoID)
END
GO

CREATE PROCEDURE dbo.usp_Message_GetPhotoID
(
	@MessageID INT
)
AS
BEGIN
	SELECT PhotoID FROM Message WHERE ID = @MessageID
END
GO

CREATE PROCEDURE dbo.usp_Message_GetList
AS
BEGIN
	SELECT Member.Name,Member.Account,Message.ID,Message.Title,Message.CreateTime 
	FROM Message Join Member ON Message.MemberID = Member.ID 
END
GO

CREATE PROCEDURE dbo.usp_Message_GetContent
(
	@MessageID INT
)
AS
BEGIN
	SELECT Member.Name,Member.Account,
	Message.ID,Message.Title,Message.Content,Message.CreateTime,Message.MemberID,Message.PhotoID
	FROM Message Join Member ON Message.MemberID = Member.ID
	WHERE Message.ID = @MessageID
END
GO

CREATE PROCEDURE dbo.usp_Message_GetMessage
(
	@MessageID INT
)
AS
BEGIN
	SELECT ID,Title,Content,MemberID,PhotoID FROM Message
	WHERE ID = @MessageID
END
GO

CREATE PROCEDURE dbo.usp_Message_GetTitle
(
	@keyword NVARCHAR(20)
)
AS
BEGIN
	SELECT Member.Name,Member.Account,Message.ID,Message.Title,Message.CreateTime
	FROM Message Join Member ON Message.MemberID = Member.ID
	WHERE Message.Title LIKE @keyword  
END
GO

CREATE PROCEDURE dbo.usp_Message_Update
(
	@NewTitle NVARCHAR(20),
	@NewContent NVARCHAR(500),
	@MessageID INT
)
AS
BEGIN
	UPDATE Message
	SET Title = @NewTitle,Content = @NewContent
	WHERE ID = @MessageID
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
