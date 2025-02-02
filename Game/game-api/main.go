package main

import (
	"game-api/database"
	"log"
	"net/http"
)

func main() {
	// Create a new ServeMux
	database.InitDB()
	// defer database.DB.Close()
	mux := http.NewServeMux()

	// Define routes
	// mux.HandleFunc("/players", playersHandler)
	// mux.HandleFunc("/items", itemsHandler)

	// Start the server
	log.Println("Server is running on :8080")
	log.Fatal(http.ListenAndServe(":8080", mux))
}
