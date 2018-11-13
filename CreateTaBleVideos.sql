CREATE TABLE Videos (
VideoID int IDENTITY(1,1) PRIMARY KEY,
Title VARCHAR(256) NOT NULL,
VideoPath VARCHAR(256),
TranscriptionPath VARCHAR(256),
DateAdded Date
)