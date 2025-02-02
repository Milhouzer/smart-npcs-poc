package handlers

import (
	"encoding/json"
	"game-api/models"
	"net/http"
)

var items = []models.Item{}

func GetItemById(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(items)
}

func AddItem(w http.ResponseWriter, r *http.Request) {
	var item models.Item
	if err := json.NewDecoder(r.Body).Decode(&item); err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	items = append(items, item)
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(item)
}

func RemoveItem(w http.ResponseWriter, r *http.Request) {
	var item models.Item
	if err := json.NewDecoder(r.Body).Decode(&item); err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	items = append(items, item)
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(item)
}
