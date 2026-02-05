public enum ItemType
{
    None,
    [DisplayName("Vũ khí chính")] MainWeapon, [DisplayName("Vũ khí phụ")] SubWeapon,
    [DisplayName("Áo giáp")] Armor, [DisplayName("Khiên")] Shield,
    [DisplayName("Mũ")] Helmet, [DisplayName("Khuyên tai")] Earrings,
    [DisplayName("Cổ vật")] Artifact, [DisplayName("Giày")] Shoes,
    [DisplayName("Đai lưng")] Belt, [DisplayName("Vòng cổ")] Necklace,
    [DisplayName("Găng tay")] Gloves, [DisplayName("Nhẫn")] Ring,
    [DisplayName("Ngọc bội")] PowerStone, [DisplayName("Dụng cụ")] Tool,
    [DisplayName("Có thể sử dụng")] Consumable, [DisplayName("Nguyên liệu")] Material,
    [DisplayName("Thông dụng")] General, [DisplayName("Đặc biệt")] Special,
    [DisplayName("Trao đổi")] Trade,
}
