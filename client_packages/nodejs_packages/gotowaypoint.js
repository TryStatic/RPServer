mp.events.add("gotowaypoint", () =>
{
	if (mp.game.invoke('0x1DD1F58F493F1DA5'))
	{ // Has waypoint set?
		let blipIterator = mp.game.invoke('0x186E5D252FA50E7D');
		let FirstInfoId = mp.game.invoke('0x1BEDE233E6CD2A1F', blipIterator);
		let NextInfoId = mp.game.invoke('0x14F96AA50D6FBEA7', blipIterator);
		for (let i = FirstInfoId; mp.game.invoke('0xA6DB27D19ECBB7DA', i) != 0; i = NextInfoId)
		{
			if (mp.game.invoke('0xBE9B0959FFD0779B', i) == 4)
			{
				let oldpos = mp.players.local.position;
				let coord = mp.game.ui.getBlipInfoIdCoord(i);

				coord.z = mp.game.gameplay.getGroundZFor3dCoord(coord.x, coord.y, i * 50, 0, false); // try calcualte Z
        		mp.players.local.position = coord;
                 
				mp.players.local.freezePosition(true);
				setTimeout(function ()
				{ // let the game load the map
					let j = 0;
					while (j <= 60 && coord.z == 0)
					{ // try to find it by trying different heights
						coord.z = mp.game.gameplay.getGroundZFor3dCoord(coord.x, coord.y, i * 25, 0, false);
						j++;
					}

					if (coord.z != 0)
					{ // if found groundZ
						mp.players.local.position = coord;
					}
					else
					{
						mp.players.local.position = oldpos;
						mp.gui.chat.push("Could not find elevation at waypoint position!");
					}
					mp.players.local.freezePosition(false);
				}, 1500);
			}
		}
	}
	else
	{
		mp.gui.chat.push("You have no waypoint set.");
	}
})