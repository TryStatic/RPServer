mp.events.add("gotowaypoint", () => {
	
    if (mp.game.invoke('0x1DD1F58F493F1DA5')) {
      let blipIterator = mp.game.invoke('0x186E5D252FA50E7D')
      let FirstInfoId = mp.game.invoke('0x1BEDE233E6CD2A1F', blipIterator)
      let NextInfoId = mp.game.invoke('0x14F96AA50D6FBEA7', blipIterator)
      for (let i = FirstInfoId; mp.game.invoke('0xA6DB27D19ECBB7DA', i) != 0; i = NextInfoId) {
        if (mp.game.invoke('0xBE9B0959FFD0779B', i) == 4 ) {
          let coord = mp.game.ui.getBlipInfoIdCoord(i)
          coord.z = mp.game.gameplay.getGroundZFor3dCoord(coord.x, coord.y, 800, 0, false)
          if(coord.z != 0 && !mp.players.local.isInAnyVehicle(true)){
            mp.players.local.position = coord
          }
          else{
            mp.gui.chat.push("Could not find elevation at waypoint position!") 
          }
        }
      }
    }
})