#region Copyright & License Information
/*
 * Copyright 2007-2015 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using OpenRA.Effects;
using OpenRA.GameRules;
using OpenRA.Mods.Common.Effects;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Warheads
{
	public class GrantUpgradeWarhead : Warhead
	{
		[UpgradeGrantedReference]
		[Desc("The upgrades to apply.")]
		public readonly string[] Upgrades = { };

		[Desc("Duration of the upgrade (in ticks). Set to 0 for a permanent upgrade.")]
		public readonly int Duration = 0;

		public readonly WDist Range = WDist.FromCells(1);

		// TODO: This can be removed after the legacy and redundant 0% = not targetable
		// assumption has been removed from the yaml definitions
		public override bool CanTargetActor(ActorInfo victim, Actor firedBy) { return true; }

		public override void DoImpact(Target target, Actor firedBy, IEnumerable<int> damageModifiers)
		{
			var actors = target.Type == TargetType.Actor ? new[] { target.Actor } :
				firedBy.World.FindActorsInCircle(target.CenterPosition, Range);

			foreach (var a in actors)
			{
				if (!IsValidAgainst(a, firedBy))
					continue;

				var um = a.TraitOrDefault<UpgradeManager>();
				if (um == null)
					continue;

				foreach (var u in Upgrades)
				{
					if (!um.AcceptsUpgrade(a, u))
						continue;

					if (Duration > 0)
						um.GrantTimedUpgrade(a, u, Duration);
					else
						um.GrantUpgrade(a, u, this);
				}
			}
		}
	}
}
