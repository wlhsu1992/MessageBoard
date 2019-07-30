CREATE PROCEDURE dbo.usp_Member_Check
(
  @Account VARCHAR(10)
)
AS
BEGIN
  SELECT Account FROM Member WHERE Account = @Account
  END
  GO

CREATE PROCEDURE dbo.usp_Member_Add
(
  @Account VARCHAR(10),
  @Password VARCHAR(10),
  @Name NVARCHAR(10)
)
AS
BEGIN
	INSERT INTO Member (Account,Password,Name)
	VALUES(@Account,@Password,@Name)
END
GO

CREATE PROCEDURE dbo.usp_Member_Login
(
	@Account VARCHAR(10),
	@Password VARCHAR(10)
)
AS
BEGIN
	SELECT ID,Account,Password,Name FROM Member 
	WHERE Account = @Account AND Password = @Password
END
GO