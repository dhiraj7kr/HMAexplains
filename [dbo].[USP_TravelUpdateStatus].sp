/****** Object:  StoredProcedure [dbo].[USP_TravelUpdateStatus]    Script Date: 17-02-2026 15:32:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[USP_TravelUpdateStatus]
(
     @status NVARCHAR(20)=NULL,
                  @journeyType NVARCHAR(20)=NULL,
                @HMATravelID NVARCHAR(20)=NULL,
                @HMATravelRequestID NVARCHAR(20)=NULL,
                @OneWaydepartureTime datetime=NULL,
                @OneWayarrivalTime datetime=NULL,
                @TwoWaydepartureTime datetime=NULL,
                @TwoWayarrivalTime datetime=NULL,
                @totalFare NVARCHAR(20)=NULL,
                                                                @responseCode NVARCHAR(max)=NULL,
                                                                @IsEmailSent BIT=NULL
																,@FromCity nvarchar(max)=null,
																@ToCity nvarchar(max)=null
                                                                )
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
                if(@status='INVOICED')
                BEGIN
                IF(@journeyType='ONE_WAY')
                BEGIN
                SELECT * FROM tblTravelRequest(nolock)
                UPDATE tblTravelRequest
                SET 
                                OneWaydepartureDate_1=@OneWaydepartureTime,
                                OneWayarrivalDate_1=@OneWayarrivalTime,
                                Status = 11,
                                TicketStatus=@status,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
                                TotalCost=@totalFare,
								FromCity=@FromCity,ToCity=@ToCity
                WHERE HMARequestId=@HMATravelID

				UPDATE tblPassengers set status=11,ModifiedDate=GETDATE(),onewaystatus=11 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (1,2,4,14)
                END
                ELSE
                BEGIN
                                UPDATE tblTravelRequest
                SET 
                                OneWaydepartureDate_1=@OneWaydepartureTime,
                                OneWayarrivalDate_1=@OneWayarrivalTime,
                                TwoWaydepartureDate_11=@TwoWaydepartureTime,
                                TwoWayarrivalDate_11=@TwoWayarrivalTime,
                                TicketStatus=@status,
                                Status = 11,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
                                TotalCost=@totalFare
								,FromCity=@FromCity,ToCity=@ToCity
                WHERE HMARequestId=@HMATravelID

				UPDATE tblPassengers set status=11,ModifiedDate=GETDATE(),onewaystatus=11,twowaystatus=11 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (1,2,4,14)
                END
                INSERT INTO [dbo].[tblUpdatedTavelRequestStatus]
           ([HMARequestId],[UpdatedStatus],[CreatedBy],[CreatedDate]
           ,[ModifiedBy],[ModifiedDate],[TicketStatus],[ResponseCode]
           ,[IsEmailSent],[Remarks])
     VALUES
           (@HMATravelID,11,'',GETDATE(),
                                   '',GETDATE(),@status,@responseCode,@IsEmailSent,'')
--UPDATE tblPassengers set status=11,ModifiedDate=GETDATE() where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2,4)

                END
    ELSE if(@status='QU0TE' or @status='Pending' )
                BEGIN
                UPDATE tblTravelRequest
                SET 
                                Status = 14,
                                ModifiedDate=GETDATE()
                WHERE HMARequestId=@HMATravelID
                INSERT INTO [dbo].[tblUpdatedTavelRequestStatus]
           ([HMARequestId],[UpdatedStatus],[CreatedBy],[CreatedDate]
           ,[ModifiedBy],[ModifiedDate],[TicketStatus],[ResponseCode]
           ,[IsEmailSent],[Remarks])
     VALUES
           (@HMATravelID,14,'',GETDATE(),
                                   '',GETDATE(),@status,@responseCode,@IsEmailSent,'')

                IF(@journeyType='ONE_WAY')
                BEGIN
UPDATE tblPassengers set status=14,onewaystatus=14,ModifiedDate=GETDATE() where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2,4)
end
else
begin 
UPDATE tblPassengers set status=14,onewaystatus=14,twowaystatus=14, ModifiedDate=GETDATE() where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2,4)
end
                END     
                ELSE if(@status='Partially Cancelled' or @status='PartiallyCancelled')
                BEGIN
                                UPDATE tblTravelRequest
                SET 
                                Status = 15,
                                ModifiedDate=GETDATE()
                WHERE HMARequestId=@HMATravelID
                INSERT INTO [dbo].[tblUpdatedTavelRequestStatus]
           ([HMARequestId],[UpdatedStatus],[CreatedBy],[CreatedDate]
           ,[ModifiedBy],[ModifiedDate],[TicketStatus],[ResponseCode]
           ,[IsEmailSent],[Remarks])
     VALUES
           (@HMATravelID,15,'',GETDATE(),
                                   '',GETDATE(),@status,@responseCode,@IsEmailSent,'')
UPDATE tblPassengers set status=6,ModifiedDate=GETDATE() where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (6)

                END 
                ELSE if(@status='Cancelled')
                BEGIN
                UPDATE tblTravelRequest
                SET 
                                Status = 9,
                                TotalCost=@totalFare,
                                TicketStatus=@status,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
								FromCity=@FromCity,ToCity=@ToCity
                WHERE HMARequestId=@HMATravelID
                INSERT INTO [dbo].[tblUpdatedTavelRequestStatus]
           ([HMARequestId],[UpdatedStatus],[CreatedBy],[CreatedDate]
           ,[ModifiedBy],[ModifiedDate],[TicketStatus],[ResponseCode]
           ,[IsEmailSent],[Remarks])
     VALUES
           (@HMATravelID,9,'',GETDATE()
                                   ,'',GETDATE(),@status,@responseCode,@IsEmailSent,'')
  IF(@journeyType='ONE_WAY')
                BEGIN
UPDATE tblPassengers set status=9,ModifiedDate=GETDATE(),onewaystatus=9 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) --and status in (6,15)
end
else
begin
UPDATE tblPassengers set status=9,ModifiedDate=GETDATE(),onewaystatus=9,twowaystatus=9 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) --and status in (6,15)
end

                END   
                else
                if(@status='CONFIRMATION_FAILED' or @status='ConfirmationFailed' or @status='Reserved')
                BEGIN
                IF(@journeyType='ONE_WAY')
                BEGIN
                SELECT * FROM tblTravelRequest(nolock)
                UPDATE tblTravelRequest
                SET 
                                OneWaydepartureDate_1=@OneWaydepartureTime,
                                OneWayarrivalDate_1=@OneWayarrivalTime,
                                Status = 7,
                                TicketStatus=@status,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
                                TotalCost=@totalFare
                WHERE HMARequestId=@HMATravelID
				UPDATE tblPassengers set status=7,ModifiedDate=GETDATE(),onewaystatus=7 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2)
                END
                ELSE
                BEGIN
                                UPDATE tblTravelRequest
                SET 
                                OneWaydepartureDate_1=@OneWaydepartureTime,
                                OneWayarrivalDate_1=@OneWayarrivalTime,
                                TwoWaydepartureDate_11=@TwoWaydepartureTime,
                                TwoWayarrivalDate_11=@TwoWayarrivalTime,
                                TicketStatus=@status,
                                Status = 7,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
                                TotalCost=@totalFare
                WHERE HMARequestId=@HMATravelID
				UPDATE tblPassengers set status=7,ModifiedDate=GETDATE(),onewaystatus=7,twowaystatus=7 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2)
                END
                INSERT INTO [dbo].[tblUpdatedTavelRequestStatus]
           ([HMARequestId],[UpdatedStatus],[CreatedBy],[CreatedDate]
           ,[ModifiedBy],[ModifiedDate],[TicketStatus],[ResponseCode]
           ,[IsEmailSent],[Remarks])
     VALUES
           (@HMATravelID,11,'',GETDATE(),
                                   '',GETDATE(),@status,@responseCode,@IsEmailSent,'')
               
                END
                if(@status='PENDING_CONFIRMATION' or @status='PendingConfirmation' or @status='ConfirmationPending')
                BEGIN
                IF(@journeyType='ONE_WAY')
                BEGIN
                SELECT * FROM tblTravelRequest(nolock)
                UPDATE tblTravelRequest
                SET 
                                OneWaydepartureDate_1=@OneWaydepartureTime,
                                OneWayarrivalDate_1=@OneWayarrivalTime,
                                Status = 16,
                                TicketStatus=@status,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
                                TotalCost=@totalFare
                WHERE HMARequestId=@HMATravelID
				UPDATE tblPassengers set status=11,ModifiedDate=GETDATE(),onewaystatus=11 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2,4)
                END
                ELSE
                BEGIN
                                UPDATE tblTravelRequest
                SET 
                                OneWaydepartureDate_1=@OneWaydepartureTime,
                                OneWayarrivalDate_1=@OneWayarrivalTime,
                                TwoWaydepartureDate_11=@TwoWaydepartureTime,
                                TwoWayarrivalDate_11=@TwoWayarrivalTime,
                                TicketStatus=@status,
                                Status = 11,
                                ModifiedDate=GETDATE(),
                                HMATravelRequestID=@HMATravelRequestID,
                                TotalCost=@totalFare
                WHERE HMARequestId=@HMATravelID

				UPDATE tblPassengers set status=11,ModifiedDate=GETDATE(),onewaystatus=11,twowaystatus=11 where TravelRequest_Id=(SELECT Top(1)Id from tblTravelRequest WHERE HMARequestId=@HMATravelID) and status in (2,4)
                END
                INSERT INTO [dbo].[tblUpdatedTavelRequestStatus]
           ([HMARequestId],[UpdatedStatus],[CreatedBy],[CreatedDate]
           ,[ModifiedBy],[ModifiedDate],[TicketStatus],[ResponseCode]
           ,[IsEmailSent],[Remarks])
     VALUES
           (@HMATravelID,11,'',GETDATE(),
                                   '',GETDATE(),@status,@responseCode,@IsEmailSent,'')


                END

END
