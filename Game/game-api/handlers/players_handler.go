package handlers

import (
	"encoding/json"
	"game-api/models"
	"net/http"
)

var players = []models.Player{}

func GetPlayerById(w http.ResponseWriter, r *http.Request) {
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(players)
}

func AddPlayer(w http.ResponseWriter, r *http.Request) {
	var player models.Player
	if err := json.NewDecoder(r.Body).Decode(&player); err != nil {
		http.Error(w, "Invalid request body", http.StatusBadRequest)
		return
	}
	players = append(players, player)
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(player)
}

func RemovePlayer(w http.ResponseWriter, r *http.Request) {
}
