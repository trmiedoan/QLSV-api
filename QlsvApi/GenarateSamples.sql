-- Genarate Samples

USE QLSV2

-- 1. Chèn dữ liệu cho 2 chuyên ngành (Khoa)
-- ID tự tăng sẽ là 1 cho CNTT và 2 cho Điện tử Viễn thông
INSERT INTO [Majors] ([MajorCode], [Name])
VALUES 
('CNTT', N'Công nghệ thông tin'),
('DTVT', N'Điện tử viễn thông');

-- 2. Chèn 20 sinh viên (15 CNTT - MajorId = 1, 5 DTVT - MajorId = 2)
INSERT INTO [Students] ([CardId], [Name], [BirthDate], [Gender], [MajorId])
VALUES
-- 15 Sinh viên Khoa CNTT (MajorId = 1)
('SV001', N'Nguyễn Văn A', '2004-01-15', N'Nam', 1),
('SV002', N'Trần Thị B', '2004-07-20', N'Nữ', 1), -- Nữ, sinh tháng 7
('SV003', N'Lê Hoàng C', '2004-03-10', N'Nam', 1),
('SV004', N'Phạm Minh D', '2004-08-25', N'Nam', 1),
('SV005', N'Hoàng Thị E', '2004-02-14', N'Nữ', 1),  -- Nữ, sinh tháng 2
('SV006', N'Nguyễn Phan F', '2004-09-05', N'Nam', 1),
('SV007', N'Vũ Thị G', '2004-11-12', N'Nữ', 1),  -- Nữ, sinh tháng 11
('SV008', N'Đặng Văn H', '2004-05-30', N'Nam', 1),
('SV009', N'Bùi Thị I', '2004-06-18', N'Nữ', 1),   -- Nữ, sinh tháng 6
('SV010', N'Ngô Văn K', '2004-10-01', N'Nam', 1),
('SV011', N'Đỗ Thị L', '2004-12-25', N'Nữ', 1),   -- Nữ, sinh tháng 12
('SV012', N'Hồ Văn M', '2004-04-05', N'Nam', 1),
('SV013', N'Mai Thị N', '2004-07-01', N'Nữ', 1),   -- Nữ, sinh tháng 7
('SV014', N'Trịnh Văn O', '2004-08-15', N'Nam', 1),
('SV015', N'Đoàn Thị P', '2004-03-22', N'Nữ', 1),

-- 5 Sinh viên Khoa Điện tử Viễn thông (MajorId = 2)
('SV016', N'Lý Văn Q', '2004-01-20', N'Nam', 2),
('SV017', N'Dương Thị R', '2004-09-10', N'Nữ', 2),
('SV018', N'Võ Văn S', '2004-05-11', N'Nam', 2),
('SV019', N'Lương Thị T', '2004-11-30', N'Nữ', 2),
('SV020', N'Nguyễn Văn U', '2004-06-15', N'Nam', 2);
