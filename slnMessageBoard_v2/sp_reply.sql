CREATE PROCEDURE dbo.usp_Reply_Add
(
	@MessageID INT,
	@MemberID INT,
	@ReplyContent NVARCHAR(50)
)
AS
BEGIN
	INSERT INTO ReplyMessage (MessageID,MemberID,ReplyContent)
	VALUES (@MessageID, @MemberID, @ReplyContent)
END
GO

CREATE PROCEDURE dbo.usp_Reply_GetContent
(
	@MessageID INT
)
AS
BEGIN
	SELECT Member.Name,Member.Account, 
           ReplyMessage.ID ,ReplyMessage.ReplyContent,ReplyMessage.ReplyTime,ReplyMessage.MemberID
    FROM Member Join ReplyMessage ON Member.ID = ReplyMessage.MemberID
    WHERE ReplyMessage.MessageID = @MessageID
END
GO

CREATE PROCEDURE dbo.usp_Reply_GetReply
(
	@ReplyID INT
)
AS
BEGIN
	SELECT ID,MessageID,ReplyContent,MemberID FROM ReplyMessage
	WHERE ID = @ReplyID
END
GO

CREATE PROCEDURE dbo.usp_Reply_Update
(
	@NewContent NVARCHAR(50),
	@ReplyID INT
)
AS
BEGIN
	UPDATE ReplyMessage
	SET ReplyContent = @NewContent
	WHERE ID = @ReplyID
END
GO

CREATE PROCEDURE dbo.usp_Reply_Delete
(
	@ReplyID INT
)
AS
BEGIN
	DELETE FROM ReplyMessage
	WHERE ID = @ReplyID
END
GO