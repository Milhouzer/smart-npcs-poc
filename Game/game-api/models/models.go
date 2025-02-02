package models

// Player represents the player data in the database
type Player struct {
	ID    int    `gorm:"primaryKey;autoIncrement" json:"id"`
	Name  string `gorm:"not null" json:"name"`
	Score int    `gorm:"default:0" json:"score"`
}

// Item represents the item data in the database
type Item struct {
	ID       int    `gorm:"primaryKey;autoIncrement" json:"id"`
	Name     string `gorm:"not null" json:"name"`
	Quantity int    `gorm:"default:0" json:"quantity"`
}

// Craft represents the craft data in the database
type Craft struct {
	ID          int    `gorm:"primaryKey;autoIncrement" json:"id"`
	Name        string `gorm:"not null" json:"name"`
	Description string `json:"description"`
	Materials   string `json:"materials"` // JSON or comma-separated list of required materials
}
