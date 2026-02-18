/****** Object:  StoredProcedure [dbo].[USP_InsertUpdateInvoiceDetails]    Script Date: 17-02-2026 15:50:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[USP_InsertUpdateInvoiceDetails]
(
			   @HMARequestId NVARCHAR(50)=NULL,  
               @FirstName NVARCHAR(25)=NULL,
			   @LastName NVARCHAR(25)=NULL,
			   @InvoiceAmount NVARCHAR(50)=NULL,
			   @InvoiceNumber NVARCHAR(50)=NULL,
			   --@InvoiceDate datetime=null,
			      @InvoiceDate NVARCHAR(50)=NULL,
			   @Status int=null,
			   @CreatedBy NVARCHAR(25)=NULL,
			   @ModifiedBy NVARCHAR(25)=NULL,
			   @FromCity NVARCHAR(200)=NULL,
			    @ToCity NVARCHAR(200)=NULL
				)
AS
BEGIN
    declare @Id int;
	declare @Travelling nvarchar(50);
	set @Travelling=(select top(1) Travelling from tbltravelrequest where Id=@HMARequestId)
	set @Id=(select top(1)Id from tblInvoiceDetails nolock where invoiceNumber=@InvoiceNumber)
	
	if(@id is null)
		begin
				insert into tblInvoiceDetails 
				(
				[PassengerId]
      ,[Status]
      ,[InvoiceNumber]
      ,[InvoiceAmount]
      ,[InvoiceDate]
      ,[IsActive]
      ,[CreatedBy]
      ,[CreatedDate]
      ,[ModifiedBy]
      ,[ModifiedDate]
      ,[HMARequestId]
	  ,FromCity
	  ,ToCity
			)
		values
			(
				(select top(1)Id from tblPassengers nolock where TravelRequest_Id=@HMARequestId and ((@Travelling='4' and GuestFirstName=@FirstName and GuestLastName=@LastName) or (@Travelling!=4 and firstName=@FirstName and lastName=@LastName))),
					@Status,
					@InvoiceNumber,
					@InvoiceAmount,
					GETDATE(),
					1,
					@CreatedBy,
					GETDATE(),
					@ModifiedBy,
					GETDATE(),
					@HMARequestId,
					@FromCity,
					@ToCity
					)

		end
	else
		begin
			declare @Paxstatus int;
			set @Paxstatus=(select status from tblPassengers where TravelRequest_Id=@HMARequestId and ((@Travelling='4' and GuestFirstName=@FirstName and GuestLastName=@LastName) or (@Travelling!=4 and firstName=@FirstName and lastName=@LastName)))
			if(@Paxstatus=11)
			begin
			update tblInvoiceDetails set 
			status=@Paxstatus,
			invoiceAmount=@InvoiceAmount,
			InvoiceDate=GETDATE(),--@InvoiceDate,
			isactive=1,
			ModifiedBy=@ModifiedBy,
			modifiedDate=GETDATE()
			where InvoiceNumber=@InvoiceNumber
			end
			else
			begin 
			if(@Paxstatus=9)
			update tblInvoiceDetails set 
			status=@Paxstatus,
			CancelledAmount=@InvoiceAmount,
			InvoiceDate=GETDATE(),--@InvoiceDate,
			isactive=1,
			ModifiedBy=@ModifiedBy,
			modifiedDate=GETDATE()
			where InvoiceNumber=@InvoiceNumber
			end
		end
END

