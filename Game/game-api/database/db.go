package database

import (
	"encoding/csv"
	"fmt"
	"game-api/models"
	"log"
	"os"
	"path/filepath"
	"reflect"
	"strconv"
	"strings"
	"time"

	"gorm.io/driver/sqlite"
	"gorm.io/gorm"
)

var DB *gorm.DB

// InitDB initializes the GORM database connection and creates tables if necessary
func InitDB() {
	var err error

	start := time.Now()
	DB, err = gorm.Open(sqlite.Open("./database/game.db"), &gorm.Config{})
	if err != nil {
		log.Fatal("Failed to connect to database:", err)
	}
	log.Printf("Database connection established (took %v).\n", time.Since(start))

	start = time.Now()
	err = DB.AutoMigrate(&models.Player{}, &models.Item{}, &models.Craft{})
	if err != nil {
		log.Fatal("Failed to migrate tables:", err)
	}
	log.Printf("Tables migrated successfully (took %v).\n", time.Since(start))

	start = time.Now()
	err = populateDataFromDirectory("./database/csv/")
	if err != nil {
		log.Printf("Error populating data from directory: %v", err)
	}
	log.Printf("Data populated from directory (took %v).\n", time.Since(start))
}

// populateDataFromDirectory processes CSV files in the specified directory and inserts data into the database
func populateDataFromDirectory(directoryPath string) error {
	return filepath.Walk(directoryPath, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			return err
		}

		if !info.IsDir() && strings.HasSuffix(info.Name(), ".csv") {
			err := populateFromCSV(path)
			if err != nil {
				log.Printf("Failed to populate data from file %s: %v", path, err)
			} else {
				log.Printf("Data populated successfully from file: %s", path)
			}
		}
		return nil
	})
}

// populateFromCSV processes a CSV file to insert rows into a table
func populateFromCSV(filePath string) error {
	file, err := os.Open(filePath)
	if err != nil {
		return err
	}
	defer file.Close()

	reader := csv.NewReader(file)
	records, err := reader.ReadAll()
	if err != nil {
		return err
	}

	if len(records) <= 1 {
		return fmt.Errorf("records are empty or only header exists: %s", filePath)
	}

	table := records[0][0]    // The first column in the header row is the table name
	columns := records[0][1:] // Remaining columns are the actual field names

	modelType, err := getModelTypeByName(table)
	if err != nil {
		return fmt.Errorf("unknown table name '%s': %v", table, err)
	}

	for i := 1; i < len(records); i++ {
		record := records[i][1:] // Skip the table column (table name is stored as the first column)

		modelInstance := reflect.New(modelType).Interface()
		err := mapRecordToModel(columns, record, modelInstance)
		if err != nil {
			return fmt.Errorf("failed to map record to model: %v", err)
		}

		if err := DB.Create(modelInstance).Error; err != nil {
			return fmt.Errorf("failed to insert record into table '%s': %v", table, err)
		}
	}

	return nil
}

// getModelTypeByName returns the struct type corresponding to a table name
func getModelTypeByName(tableName string) (reflect.Type, error) {
	switch tableName {
	case "players":
		return reflect.TypeOf(models.Player{}), nil
	case "items":
		return reflect.TypeOf(models.Item{}), nil
	case "crafts":
		return reflect.TypeOf(models.Craft{}), nil
	default:
		return nil, fmt.Errorf("unknown table name '%s'", tableName)
	}
}

// mapRecordToModel maps a CSV record to a struct instance
func mapRecordToModel(columns []string, record []string, model interface{}) error {
	modelValue := reflect.ValueOf(model).Elem()

	for i, column := range columns {
		field := modelValue.FieldByName(strings.Title(column))
		if !field.IsValid() {
			return fmt.Errorf("unknown field '%s' in model", column)
		}

		if !field.CanSet() {
			return fmt.Errorf("cannot set field '%s' in model", column)
		}

		value := record[i]
		switch field.Kind() {
		case reflect.Int:
			intValue, err := strconv.Atoi(value)
			if err != nil {
				return fmt.Errorf("failed to convert '%s' to int: %v", value, err)
			}
			field.SetInt(int64(intValue))
		case reflect.String:
			field.SetString(value)
		default:
			return fmt.Errorf("unsupported field type '%s' for field '%s'", field.Kind(), column)
		}
	}

	return nil
}
