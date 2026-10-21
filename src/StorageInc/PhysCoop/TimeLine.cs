using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;

namespace PhysCoop;

public class TimeLine
{
	public float PeriodClock;

	public int PeriodLength;

	public float DeliverClock;

	public int DeliverLength = 2400;

	public int NextGate;

	private GameplayScreen myScreen;

	private int numberOfPlayers;

	private int min_goods;

	private int max_goods;

	public float OrderOutNotifyRotation = 0f;

	private bool periodFlip = false;

	private bool flipSwitched = false;

	public float FlipAlpha = 1f;

	public float PlayerRatioMod = 1f;

	public TimeLine(GameplayScreen p_screen, int p_preplaced_goods)
	{
		myScreen = p_screen;
		NextGate = myScreen.rnd.Next(myScreen.InGates.Length);
		min_goods = (int)((float)p_preplaced_goods * 1.2f);
		max_goods = (int)((float)p_preplaced_goods * 1.25f);
		PeriodLength = (int)(8400f * ((float)min_goods / 48f));
	}

	public void Update(SoundBank soundBank)
	{
		OrderOutNotifyRotation -= 0.05f;
		MathHelper.WrapAngle(OrderOutNotifyRotation);
		numberOfPlayers = 0;
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				numberOfPlayers++;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		PlayerRatioMod = (float)numberOfPlayers * 0.54f + 0.4f;
		switch (myScreen.ScreenManager.GameSaveInfo.GameDifficulty)
		{
		case 0:
			PlayerRatioMod *= 0.75f;
			break;
		case 1:
			PlayerRatioMod *= 0.9f;
			break;
		case 3:
			PlayerRatioMod *= 1.15f;
			break;
		}
		if (!myScreen.TutorialPeriodPause)
		{
			PeriodClock += PlayerRatioMod;
		}
		if (PeriodClock >= (float)PeriodLength)
		{
			periodFlip = true;
			flipSwitched = false;
			PeriodClock = 0f;
			soundBank.PlayCue("NewPeriod");
			myScreen.TutorialSkippedPeriod = true;
		}
		if (periodFlip)
		{
			if (!flipSwitched)
			{
				FlipAlpha -= 0.03f;
				if (FlipAlpha <= 0f)
				{
					for (int i = 0; i < myScreen.goodsList.Count; i++)
					{
						myScreen.goodsList[i].NewPeriod();
					}
					flipSwitched = true;
					FlipAlpha = 0f;
				}
			}
			else
			{
				FlipAlpha += 0.03f;
				if (FlipAlpha >= 1f)
				{
					periodFlip = false;
					FlipAlpha = 1f;
				}
			}
		}
		float num = 1f;
		if (myScreen.goodsList.Count < min_goods)
		{
			num = (float)Math.Pow((float)min_goods / (float)myScreen.goodsList.Count, 6.0);
		}
		else if (myScreen.goodsList.Count > max_goods)
		{
			num = (float)Math.Pow((float)max_goods / (float)myScreen.goodsList.Count, 6.0);
		}
		num = MathHelper.Clamp(num, 0.2f, 3f);
		if (!myScreen.TutorialDeliverPause)
		{
			if (!myScreen.DeliverPause)
			{
				DeliverClock += PlayerRatioMod * num;
			}
			else if (PeriodClock > 480f)
			{
				myScreen.DeliverPause = false;
			}
		}
		if (DeliverClock >= (float)DeliverLength)
		{
			DeliverClock = 0f;
			if (myScreen.InGates[NextGate].IsBlocked || myScreen.InGates[NextGate].myDelivery != null)
			{
				myScreen.LooseLife();
			}
			else
			{
				soundBank.PlayCue("GoodsSlide");
				myScreen.InGates[NextGate].NewArrival(myScreen.InGates[NextGate]);
			}
			NextGate++;
			if (NextGate >= myScreen.InGates.Length)
			{
				NextGate = 0;
			}
		}
	}
}
