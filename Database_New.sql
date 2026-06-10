-- =======================================
-- TẠO CƠ SỞ DỮ LIỆU
-- =======================================
CREATE DATABASE QuanLyVatLieuXayDung;
GO
USE [QuanLyVatLieuXayDung];
GO

-- =======================================
-- 1. KHỞI TẠO CẤU TRÚC CÁC BẢNG (TABLES)
-- =======================================

-- Loại vật liệu --
CREATE TABLE LoaiVatLieu (
    ID char(10) PRIMARY KEY,
    DisplayName nvarchar (max)
);
GO

-- Nhà cung cấp --
CREATE TABLE NhaCungCap (
    ID char(10) PRIMARY KEY NOT NULL,
    DisplayName nvarchar (max) NOT NULL,
    Address nvarchar (max) NULL,
    Phone varchar(15) NULL,
    Email nvarchar (50) NULL,
    MoreInfo nvarchar (max) NULL,
    ContractDate DateTime
);
GO

-- Khách hàng --
CREATE TABLE KhachHang (
    ID char(10) PRIMARY KEY NOT NULL,
    DisplayName nvarchar (max) NOT NULL,
    Address nvarchar (max) NULL,
    Phone varchar(15) NULL,
    Email nvarchar (50) NULL,
    MoreInfo nvarchar (max) NULL,
    ContractDate DateTime
);
GO

-- Vật liệu --
CREATE TABLE VatLieu (
    ID char(10) PRIMARY KEY,
    DisplayName nvarchar (max),
    IDLoaiVatLieu char(10) NOT NULL,
    DonViTinh nvarchar (50),
    QRCode nvarchar (max),
    Barcode nvarchar (max)
);
GO

-- Quyền truy cập của người dùng (App Role) --
CREATE TABLE NguoiDungRole (
    ID char(10) PRIMARY KEY,
    DisplayName nvarchar (max)
);
GO

-- Người dùng (Tài khoản App) --
CREATE TABLE NguoiDung (
    ID char(10) PRIMARY KEY,
    DisplayName nvarchar (max),
    UserName nvarchar (100),
    Password nvarchar (max),
    IDRole char(10) NOT NULL
);
GO

-- Phiếu nhập --
CREATE TABLE PhieuNhap (
    ID char(10) PRIMARY KEY,
    IDNhaCungCap char(10) NULL,
    DateInput datetime,
    Status NVARCHAR(MAX)
);
GO
-- Chi tiết phiếu nhập --
CREATE TABLE ChiTietPhieuNhap (
    ID char(10) PRIMARY KEY,
    IDObject char(10) NOT NULL,
    IDINput char(10) NOT NULL,
    Counts int,
    PriceInput float DEFAULT 0,
    PriceOutput float DEFAULT 0,
    Status nvarchar (max)
);
GO
   


-- Phiếu xuất (Đóng vai trò Hóa Đơn) --
CREATE TABLE PhieuXuat (
    ID char(10) PRIMARY KEY,
    DateOutput datetime,
    IDUser char(10), -- Người lập hóa đơn
    Total float DEFAULT 0, -- Tổng tiền
    IDCustomer char(10) NOT NULL,
    Status nvarchar (max),
    ChietKhau float DEFAULT 0,
    SoTienDaThanhToan float DEFAULT 0,
    PhuongThucThanhToan nvarchar(max)
);
GO

-- Chi tiết phiếu xuất (Đóng vai trò Chi tiết Hóa Đơn) --
CREATE TABLE ChiTietPhieuXuat (
    ID char(10) PRIMARY KEY,
    IDObject char(10) NOT NULL,
    IDOutput char(10) NOT NULL,
    Counts int,
    Price float DEFAULT 0 -- Giá bán
);
GO

CREATE TABLE LichSuGiaVatLieu (
    ID int IDENTITY(1,1) PRIMARY KEY,
    IDVatLieu char(10) NOT NULL,
    GiaCu float,
    GiaMoi float NOT NULL,
    NgayThayDoi datetime DEFAULT getdate(),
    NguoiThayDoi nvarchar(100),
    FOREIGN KEY (IDVatLieu) REFERENCES VatLieu(ID)
);

CREATE TABLE PhieuThu (
    ID char(10) PRIMARY KEY,
    IDKhachHang char(10) NOT NULL,
    NgayThu datetime DEFAULT getdate(),
    SoTien float DEFAULT 0,
    NoiDung nvarchar(max),
    IDUser char(10) NULL,
    FOREIGN KEY (IDKhachHang) REFERENCES KhachHang(ID),
    FOREIGN KEY (IDUser) REFERENCES NguoiDung(ID)
);

CREATE TABLE PhieuChi (
    ID char(10) PRIMARY KEY,
    IDNhaCungCap char(10) NOT NULL,
    NgayChi datetime DEFAULT getdate(),
    SoTien float DEFAULT 0,
    NoiDung nvarchar(max),
    IDUser char(10) NULL,
    FOREIGN KEY (IDNhaCungCap) REFERENCES NhaCungCap(ID),
    FOREIGN KEY (IDUser) REFERENCES NguoiDung(ID)
);


-- =======================================
-- 2. TẠO RÀNG BUỘC (CONSTRAINTS) VÀ KHÓA NGOẠI
-- =======================================

ALTER TABLE [dbo].[NguoiDung] ADD CONSTRAINT UC_UserName UNIQUE NONCLUSTERED ([UserName] ASC);

ALTER TABLE [dbo].[PhieuNhap] ADD CONSTRAINT DF_PhieuNhap_Date DEFAULT (getdate()) FOR [DateInput];

ALTER TABLE [dbo].[PhieuXuat] ADD CONSTRAINT DF_PhieuXuat_Date DEFAULT (getdate()) FOR [DateOutput];

ALTER TABLE [dbo].[ChiTietPhieuNhap] ADD CONSTRAINT DF_CTPN_Counts DEFAULT ((0)) FOR [Counts];

ALTER TABLE [dbo].[ChiTietPhieuXuat] ADD CONSTRAINT DF_CTPX_Counts DEFAULT ((0)) FOR [Counts];
GO

-- Khóa ngoại Vật liệu --
ALTER TABLE [dbo].[VatLieu] WITH CHECK ADD FOREIGN KEY([IDLoaiVatLieu]) REFERENCES [dbo].[LoaiVatLieu] ([ID]);

-- Khóa ngoại Người dùng --
ALTER TABLE [dbo].[NguoiDung] WITH CHECK ADD FOREIGN KEY([IDRole]) REFERENCES [dbo].[NguoiDungRole] ([ID]);

-- Khóa ngoại Phiếu kho & Hóa đơn --
ALTER TABLE [dbo].[PhieuNhap] WITH CHECK ADD FOREIGN KEY([IDNhaCungCap]) REFERENCES [dbo].[NhaCungCap] ([ID]);

ALTER TABLE [dbo].[ChiTietPhieuNhap] WITH CHECK ADD FOREIGN KEY([IDObject]) REFERENCES [dbo].[VatLieu] ([ID]);

ALTER TABLE [dbo].[ChiTietPhieuNhap] WITH CHECK ADD FOREIGN KEY([IDINput]) REFERENCES [dbo].[PhieuNhap] ([ID]);

ALTER TABLE [dbo].[ChiTietPhieuXuat] WITH CHECK ADD FOREIGN KEY([IDObject]) REFERENCES [dbo].[VatLieu] ([ID]);

ALTER TABLE [dbo].[ChiTietPhieuXuat] WITH CHECK ADD FOREIGN KEY([IDOutput]) REFERENCES [dbo].[PhieuXuat] ([ID]);

ALTER TABLE [dbo].[PhieuXuat] WITH CHECK ADD FOREIGN KEY([IDCustomer]) REFERENCES [dbo].[KhachHang] ([ID]);

ALTER TABLE [dbo].[PhieuXuat] WITH CHECK ADD FOREIGN KEY([IDUser]) REFERENCES [dbo].[NguoiDung] ([ID]);
GO

-- =======================================
-- 4. CHÈN DỮ LIỆU MẪU (INSERT DATA) GỐC
-- =======================================

-- Quyền hệ thống (Roles) --
INSERT INTO
    NguoiDungRole
VALUES ('AM', N'Admin hệ thống'),
    ('BH', N'Bộ phận Bán hàng'),
    ('TK', N'Bộ phận Kho');
GO

-- Tài khoản người dùng (Users) --
INSERT INTO
    NguoiDung (
        ID,
        DisplayName,
        UserName,
        Password,
        IDRole
    )
VALUES (
        'NV002',
        N'Châu Khải Vinh',
        'VinhCK',
        'VinhCK@123',
        'AM'
    ),
    (
        'NV003',
        N'Nguyễn Hữu Lộc',
        'LocNH',
        'LocNH@123',
        'BH'
    ),
    (
        'NV004',
        N'Phạm Hữu Vinh',
        'VinhPH',
        'VinhPH@123',
        'BH'
    ),
    (
        'NV005',
        N'Ngô Thuận Văn',
        'VanNT',
        'VanNT@123',
        'TK'
    );
GO

-- Loại vật liệu --
INSERT INTO
    LoaiVatLieu (ID, DisplayName)
VALUES ('LVL001', N'Cát'),
    ('LVL002', N'Đá'),
    ('LVL003', N'Xi măng'),
    ('LVL004', N'Sắt thép'),
    ('LVL005', N'Gạch xây'),
    ('LVL006', N'Sơn nước');

-- Nhà cung cấp --
INSERT INTO
    NhaCungCap
VALUES (
        'NCC001',
        N'Công ty Cổ phần Thép Hòa Phát',
        N'KCN Hiệp Phước, Nhà Bè, TP. Hồ Chí Minh',
        '02837800888',
        'sales@hoaphat.com.vn',
        N'Nhà cung cấp thép xây dựng, thép ống chủ lực',
        '2020-03-15'
    ),
    (
        'NCC002',
        N'Tập đoàn Hoa Sen',
        N'183 Nguyễn Văn Trỗi, Phường 10, Phú Nhuận, TP. Hồ Chí Minh',
        '02839977997',
        'info@hoasengroup.vn',
        N'Chuyên cung cấp tôn cán nóng, tôn mạ màu và ống nhựa',
        '2021-05-20'
    ),
    (
        'NCC003',
        N'Công ty Xi măng Vicem Hà Tiên',
        N'360 Xa lộ Hà Nội, Phường Phước Long A, TP. Thủ Đức, TP. HCM',
        '02838966776',
        'lienhe@vicemhatien.com.vn',
        N'Cung cấp xi măng bao và xi măng rời chất lượng cao',
        '2019-11-12'
    ),
    (
        'NCC004',
        N'Tổng Công ty Thủy tinh và Gốm xây dựng Viglacera',
        N'Tòa nhà Viglacera, Từ Liêm, Hà Nội',
        '02435582888',
        'info@viglacera.com.vn',
        N'Nhà cung cấp gạch ốp lát, thiết bị vệ sinh, kính xây dựng',
        '2022-01-10'
    ),
    (
        'NCC005',
        N'Công ty TNHH Thép Vina Kyoei',
        N'KCN Phú Mỹ I, Thị xã Phú Mỹ, Bà Rịa - Vũng Tàu',
        '02543876255',
        'sales@vinakyoei.com.vn',
        N'Cung cấp cốt thép bê tông, thép cuộn chất lượng Nhật Bản',
        '2020-08-05'
    ),
    (
        'NCC006',
        N'Công ty Cổ phần Gạch Đồng Tâm',
        N'Số 7, Khu Phố 6, Thị Trấn Bến Lức, Tỉnh Long An',
        '02723872233',
        'dongtam@dongtam.com.vn',
        N'Chuyên gạch ceramic, gạch granite và sơn nước cao cấp',
        '2021-09-18'
    ),
    (
        'NCC007',
        N'Công ty TNHH Panasonic Life Solutions Việt Nam',
        N'KCN Thăng Long, Đông Anh, Hà Nội',
        '02439550111',
        'support.plsvn@vn.panasonic.com',
        N'Cung cấp thiết bị điện dân dụng, công tắc, ổ cắm, đèn LED',
        '2023-04-25'
    ),
    (
        'NCC008',
        N'Công ty Cổ phần Nhựa Bình Minh',
        N'240 Hậu Giang, Phường 9, Quận 6, TP. Hồ Chí Minh',
        '02839690973',
        'binhminh@binhminhplastic.com.vn',
        N'Cung cấp ống nhựa PVC-U, HDPE, PP-R cho hệ thống cấp thoát nước',
        '2018-06-30'
    ),
    (
        'NCC009',
        N'Công ty TNHH Sơn TOA Việt Nam',
        N'KCN Tân Đông Hiệp A, Dĩ An, Bình Dương',
        '02743742242',
        'toamarketing@toagroup.com.vn',
        N'Nhà cung cấp sơn công nghiệp, sơn kiến trúc và chất chống thấm',
        '2022-10-15'
    ),
    (
        'NCC010',
        N'Công ty Cổ phần Đá Thạch Anh Cao Cấp VICOSTONE',
        N'KCN Bắc Phú Cát, Thạch Thất, Hà Nội',
        '02433685826',
        'info@vicostone.com',
        N'Cung cấp đá nhân tạo gốc thạch anh cao cấp cho mặt bếp, quầy bar',
        '2024-02-28'
    );
GO

-- Khách hàng --
INSERT INTO
    KhachHang
VALUES (
        'KH001',
        N'Nguyễn Văn An',
        N'125/4 Nguyễn Văn Thương, Bình Thạnh, TP. HCM',
        '0908123456',
        'nguyenvanan.vlxd@gmail.com',
        N'Chủ nhà tự mua vật tư xây nhà phố 3 tầng',
        '2023-01-15'
    ),
    (
        'KH002',
        N'Trần Thị Bích',
        N'482 Lê Văn Sỹ, Phường 14, Quận 3, TP. HCM',
        '0913987654',
        'bichtran.realtor@gmail.com',
        N'Khách hàng thân thiết, chuyên mua sơn và gạch sửa nhà',
        '2022-11-20'
    ),
    (
        'KH003',
        N'Lê Hoàng Cường',
        N'74 Lộ Tẻ, Tân Tạo, Quận Bình Tân, TP. HCM',
        '0934556677',
        'lehoangcuong90@gmail.com',
        N'Thầu xây dựng nhỏ lẻ khu vực Bình Tân',
        '2024-05-12'
    ),
    (
        'KH004',
        N'Phạm Minh Đức',
        N'12 Đường số 4, kDC Chu Văn An, Bình Thạnh, TP. HCM',
        '0987112233',
        'phamminhduc.kstr@gmail.com',
        N'Kỹ sư thiết kế, thường giới thiệu chủ nhà đến mua vật tư',
        '2021-08-18'
    ),
    (
        'KH005',
        N'Vũ Thị Hồng Dung',
        N'188/12 Quốc lộ 1K, Linh Xuân, TP. Thủ Đức, TP. HCM',
        '0902334455',
        'vuhongdung.kd@gmail.com',
        N'Mua thiết bị vệ sinh và gạch ốp lát hoàn thiện chung cư',
        '2023-09-05'
    ),
    (
        'KH006',
        N'Hoàng Đình Hải',
        N'53 Nguyễn Du, Phường Bến Nghé, Quận 1, TP. HCM',
        '0918556677',
        'hoangdinhhai.arc@gmail.com',
        N'Kiến trúc sư tự do, khách hàng VIP mảng đá thạch anh',
        '2022-04-12'
    ),
    (
        'KH007',
        N'Đỗ Minh Long',
        N'95/2 Nguyễn Ảnh Thủ, Quận 12, TP. HCM',
        '0975667788',
        'dominhlong1985@gmail.com',
        N'Chủ thầu thợ hồ, thường lấy sắt thép và xi măng',
        '2023-07-30'
    ),
    (
        'KH008',
        N'Bùi Tuyết Mai',
        N'31 Thảo Điền, Phường Thảo Điền, TP. Thủ Đức, TP. HCM',
        '0945112233',
        'tuyetmai.bui@gmail.com',
        N'Chủ nhà biệt thự, đòi hỏi vật liệu hoàn thiện cao cấp',
        '2024-03-14'
    ),
    (
        'KH009',
        N'Phan Thanh Nam',
        N'204 Phùng Hưng, Phường 13, Quận 5, TP. HCM',
        '0963889900',
        'phanthanhnam.vlxd@gmail.com',
        N'Mua sỉ ống nước và thiết bị điện về làm công trình',
        '2020-06-25'
    ),
    (
        'KH010',
        N'Ngô Quốc Tuấn',
        N'88 Đường số 9, KDC Him Lam, Quận 7, TP. HCM',
        '0909445566',
        'tuango.decor@gmail.com',
        N'Nhà thầu chuyên nhận sửa chữa, cải tạo nội thất chung cư',
        '2024-02-28'
    );
GO

-- Vật liệu --
INSERT INTO
    VatLieu (
        ID,
        DisplayName,
        IDLoaiVatLieu,
        QRCode,
        Barcode,
        DonViTinh
    )
VALUES (
        'VL001',
        N'Cát san lấp',
        'LVL001',
        'QR001',
        'BAR001',
        N'Khối'
    ),
    (
        'VL002',
        N'Đá 1x2 Xanh',
        'LVL002',
        'QR002',
        'BAR002',
        N'Khối'
    ),
    (
        'VL003',
        N'Xi măng Hà Tiên Đa Dụng',
        'LVL003',
        'QR003',
        'BAR003',
        N'Bao'
    ),
    (
        'VL004',
        N'Sắt cuộn Pomina Phi 8',
        'LVL004',
        'QR004',
        'BAR004',
        N'Kg'
    );
GO

-- Phiếu nhập --
INSERT INTO
    PhieuNhap (ID, IDNhaCungCap, DateInput,Status)
VALUES (
        'PN001',
        'NCC001',
        '2023-10-01 08:30:00',
        N'Đã hoàn thành'
    ),
    (
        'PN002',
        'NCC003',
        '2023-10-15 09:00:00',
        N'Đã hoàn thành'
    );
GO

-- Chi tiết phiếu nhập --
INSERT INTO
    ChiTietPhieuNhap (
        ID,
        IDObject,
        IDINput,
        Counts,
        PriceInput,
        PriceOutput
    )
VALUES (
        'CTN001',
        'VL001',
        'PN001',
        500,
        120000,
        150000
    ),
    (
        'CTN002',
        'VL002',
        'PN001',
        300,
        250000,
        300000
    ),
    (
        'CTN003',
        'VL003',
        'PN002',
        1000,
        85000,
        95000
    ),
    (
        'CTN004',
        'VL004',
        'PN002',
        5000,
        15000,
        17500
    );
GO

-- Phiếu Xuất (Đã chèn rõ tên cột để khớp với dữ liệu gốc của bạn) --
INSERT INTO
    PhieuXuat (ID, DateOutput, Total, IDCustomer, Status, SoTienDaThanhToan, PhuongThucThanhToan)
VALUES (
        'PX001',
        '2023-11-05 10:15:00',
        0,
        'KH001',
        N'Đã xuất',
        0,
        N'Tiền mặt'
    ),
    (
        'PX002',
        '2023-11-20 14:20:00',
        0,
        'KH002',
        N'Chưa xuất',
        0,
        N'Chuyển khoản'
    );
GO

-- Chi tiết phiếu xuất (Đã chèn rõ tên cột để khớp với dữ liệu gốc của bạn) --
INSERT INTO
    ChiTietPhieuXuat (
        ID,
        IDObject,
        IDOutput,
        Counts,
        Price
    )
VALUES (
        'CTX001',
        'VL001',
        'PX001',
        100,
        150000
    ), -- Bán 100 khối cát
    (
        'CTX002',
        'VL003',
        'PX001',
        200,
        95000
    ), -- Bán 200 bao xi măng
    (
        'CTX003',
        'VL004',
        'PX002',
        1500,
        17500
    );
-- Bán 1500 kg sắt
GO

-- ==========================================
-- 5. BẢO MẬT PHÂN QUYỀN (SQL NATIVE RBAC) GỐC
-- ==========================================

-- 5.1. Tạo Logins (Tài khoản kết nối Server)
CREATE LOGIN [Login_Admin_Vinh] WITH PASSWORD = 'VinhCK@123';

CREATE LOGIN [Login_Sales_Loc] WITH PASSWORD = 'LocNH@123';

CREATE LOGIN [Login_Sales_Vinh] WITH PASSWORD = 'VinhPH@123';

CREATE LOGIN [Login_Warehouse_Van] WITH PASSWORD = 'VanNT@123';

CREATE LOGIN [Login_Warehouse_Vinh] WITH PASSWORD = 'VinhLQ@123';
GO

-- 5.2. Tạo Users (Tài khoản truy cập Database)
CREATE USER [User_Admin_Vinh] FOR LOGIN [Login_Admin_Vinh];

CREATE USER [User_Sales_Loc] FOR LOGIN [Login_Sales_Loc];

CREATE USER [User_Sales_Vinh] FOR LOGIN [Login_Sales_Vinh];

CREATE USER [User_Warehouse_Van] FOR LOGIN [Login_Warehouse_Van];

CREATE USER [User_Warehouse_Vinh] FOR LOGIN [Login_Warehouse_Vinh];
GO

-- 5.3. Tạo Database Roles
CREATE ROLE [Role_Sales];

CREATE ROLE [Role_Warehouse];
GO
-- 5.4. GÁN QUYỀN CHO BỘ PHẬN BÁN HÀNG (SALES)
GRANT SELECT ON LoaiVatLieu TO [Role_Sales];

GRANT SELECT ON VatLieu TO [Role_Sales];

GRANT SELECT, INSERT, UPDATE ON KhachHang TO [Role_Sales];

-- Bán hàng được quyền thao tác trên Phiếu Xuất (Hóa Đơn)
GRANT SELECT, INSERT, UPDATE ON PhieuXuat TO [Role_Sales];

GRANT SELECT, INSERT, UPDATE ON ChiTietPhieuXuat TO [Role_Sales];

DENY DELETE ON PhieuXuat TO [Role_Sales];

DENY DELETE ON ChiTietPhieuXuat TO [Role_Sales];

-- Cấm Bán hàng đụng vào Phiếu Nhập
DENY SELECT, INSERT, UPDATE, DELETE ON PhieuNhap TO [Role_Sales];

DENY SELECT, INSERT, UPDATE, DELETE ON ChiTietPhieuNhap TO [Role_Sales];
GO

-- 5.5. GÁN QUYỀN CHO BỘ PHẬN KHO (WAREHOUSE)
GRANT SELECT, INSERT, UPDATE ON LoaiVatLieu TO [Role_Warehouse];

GRANT SELECT, INSERT, UPDATE ON VatLieu TO [Role_Warehouse];

GRANT SELECT, INSERT, UPDATE ON NhaCungCap TO [Role_Warehouse];

GRANT SELECT, INSERT, UPDATE, DELETE ON PhieuNhap TO [Role_Warehouse];

GRANT SELECT, INSERT, UPDATE, DELETE ON ChiTietPhieuNhap TO [Role_Warehouse];

-- Kho được xem Phiếu Xuất để soạn hàng nhưng không được lập hay xóa
GRANT SELECT ON PhieuXuat TO [Role_Warehouse];

GRANT SELECT ON ChiTietPhieuXuat TO [Role_Warehouse];

DENY INSERT, UPDATE, DELETE ON PhieuXuat TO [Role_Warehouse];

DENY INSERT, UPDATE, DELETE ON ChiTietPhieuXuat TO [Role_Warehouse];
GO

-- 5.6. ĐƯA USERS VÀO ROLES TƯƠNG ỨNG
-- Gán quyền Admin (Toàn quyền)
ALTER ROLE [db_owner] ADD MEMBER [User_Admin_Vinh];
-- Gán quyền Bán Hàng
ALTER ROLE [Role_Sales] ADD MEMBER [User_Sales_Loc];

ALTER ROLE [Role_Sales] ADD MEMBER [User_Sales_Vinh];
-- Gán quyền Kho
ALTER ROLE [Role_Warehouse] ADD MEMBER [User_Warehouse_Van];

ALTER ROLE [Role_Warehouse] ADD MEMBER [User_Warehouse_Vinh];
GO
