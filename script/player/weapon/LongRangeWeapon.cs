/*************************************************************************/
/*  LongRangeWeapon.cs                                                   */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/dwkim263/DungeonPath/wiki              */
/*************************************************************************/
/* Copyright (c) 2018-2019 Dong Won Kim.                                 */
/*                                                                       */
/* Permission is hereby granted, free of charge, to any person obtaining */
/* a copy of this software and associated documentation files (the       */
/* "Software"), to deal in the Software without restriction, including   */
/* without limitation the rights to use, copy, modify, merge, publish,   */
/* distribute, sublicense, and/or sell copies of the Software, and to    */
/* permit persons to whom the Software is furnished to do so, subject to */
/* the following conditions:                                             */
/*                                                                       */
/* The above copyright notice and this permission notice shall be        */
/* included in all copies or substantial portions of the Software.       */
/*                                                                       */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,       */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF    */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.*/
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY  */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,  */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE     */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                */
/*************************************************************************/
using Godot;
using System;

public class LongRangeWeapon : Weapon
{
	//Set MaximumAp
    public override void SetMaximumAp(int level)
    {
		double results = (Constants.BASE_HP +
		(Constants.MAXIMUM_POSSIBLE_HP - Constants.BASE_HP) *
		level / Constants.MAXIMUM_LEVEL) / 10;
		MaximumAp = (int)Math.Floor(results);
    }

    //Set MinimumAp
    public override void SetMinimumAp(int level)
    {
		double results = (Constants.BASE_HP +
		(Constants.MAXIMUM_POSSIBLE_HP - Constants.BASE_HP) *
		level / Constants.MAXIMUM_LEVEL) / 15;
        MinimumAp = (int)Math.Floor(results);
    }

    //Random number between MinimumAp and MaximumAp
	public override int GetAttackPoints()
    {
        return (int)Constants.RandRand(MinimumAp, MaximumAp);
    }
}
