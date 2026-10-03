using System.Collections.Generic;

namespace PhysCoop;

public class MapContainer
{
	private class MapDifficultyComparer : IComparer<MapItem>
	{
		public int Compare(MapItem map1, MapItem map2)
		{
			if (map1.Difficulty == map2.Difficulty)
			{
				return map1.Id.CompareTo(map2.Id);
			}
			return map1.Difficulty.CompareTo(map2.Difficulty);
		}
	}

	private class MapIndexComparer : IComparer<MapItem>
	{
		public int Compare(MapItem map1, MapItem map2)
		{
			return map1.Id.CompareTo(map2.Id);
		}
	}

	private class MapNameComparer : IComparer<MapItem>
	{
		public int Compare(MapItem map1, MapItem map2)
		{
			return map1.Name.CompareTo(map2.Name);
		}
	}

	public class MapItem
	{
		public int Id;

		public string Name;

		public int Difficulty;

		public bool LockedOnTrial;

		public string Description;

		public MapItem(int id, string name, int difficulty, bool lockedOnTrial, string description)
		{
			Id = id;
			Name = name;
			Difficulty = difficulty;
			LockedOnTrial = lockedOnTrial;
			Description = description;
		}
	}

	public List<MapItem> Maps = new List<MapItem>();

	public int ListMapIndex = 0;

	public MapContainer()
	{
		Maps.Add(new MapItem(1, "Tutorial", 0, lockedOnTrial: false, "Learn how to play."));
		Maps.Add(new MapItem(2, "OverTime", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(3, "Easy Going", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(4, "Easy Trial Map", 0, lockedOnTrial: false, "Beginners map with lots\nof extra space and few\ngoods."));
		Maps.Add(new MapItem(5, "Rooms", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(6, "PayCheck", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(7, "One Way", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(8, "Organize", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(9, "Open for Business", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(10, "Crates", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(11, "Too Many To Handle", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(12, "Cakewalk", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(13, "Normal Trial Map", 0, lockedOnTrial: false, "Not that many goods to\nkeep track of, but with\na tight map layout."));
		Maps.Add(new MapItem(14, "Space", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(15, "Separation", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(16, "A Bit Random", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(17, "Confused", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(18, "Sections", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(19, "Detours", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(20, "Circular", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(21, "Personal Storage", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(22, "Ahead of Time", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(23, "Warehouse Inc", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(24, "Tough Symmetry", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(25, "Pockets", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(26, "Out of Space", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(27, "Day Workers", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(28, "We Move Boxes", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(29, "Spread Out", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(30, "Structured Rows", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(31, "1 in 4 out", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(32, "Packages", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(33, "Temporary Storage", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(34, "9 to 5", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(35, "Big and Easy", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(36, "Area 33", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(37, "A Job 4 Us", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(38, "Boxing", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(39, "Another Day", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(40, "Backup", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(41, "Inner Space", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(42, "Central Storage", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(43, "Lines", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(44, "Pillar Box", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(45, "Not Optimized", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(46, "Mileage", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(47, "We Need a Raise", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(48, "Free Space", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(49, "Replace", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(50, "Only Rooms", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(51, "What Goes Around", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(52, "Outa Space", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(53, "Sort Of Boxes", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(54, "Pockets", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(55, "Open For Business", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(56, "Paths", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(57, "Hurry Up", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(58, "Busy Hour", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(59, "Split", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(60, "Crosses", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(61, "Rush Hour", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(62, "Gate Blocks", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(63, "LiftaDroppa", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(64, "No Break", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(65, "In The Middle", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(66, "Left To Right", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(67, "Orders", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(68, "No Room 4 Mistakes", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(69, "Monday Morning", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(70, "Section 7", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(71, "Bend", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(72, "Too Narrow", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(73, "A Messy Start", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(74, "Sort It Out", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(75, "Room For All", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(76, "Tight Spots", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(77, "U Turn", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(78, "Tough Spots", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(79, "Twister", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(80, "Up And Down", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(81, "Out Side", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(82, "Teeth", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(83, "Hell Circle", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(84, "Third Shift", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(85, "Stairs", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(86, "Busy Hour", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(87, "Splitter", 1, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(88, "No Rules", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(89, "Out For Lunch", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(90, "Last Minute", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(91, "A Lot", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(92, "Layers", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(93, "Hard Work", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(94, "Precision", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(95, "Against The Odds", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(96, "Pin Point", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(97, "Empty Room", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(98, "Arrow", 2, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(99, "Back Room", 3, lockedOnTrial: true, ""));
		Maps.Add(new MapItem(100, "Pyramid", 3, lockedOnTrial: true, ""));
	}

	public void SortByIndex()
	{
		Maps.Sort(new MapIndexComparer());
	}

	public void SortByDifficulty()
	{
		Maps.Sort(new MapDifficultyComparer());
	}

	public void SortByName()
	{
		Maps.Sort(new MapNameComparer());
	}
}
