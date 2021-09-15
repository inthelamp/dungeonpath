/*************************************************************************/
/*  LongRangeWeapon.cs                                                   */
/*************************************************************************/
/*                       This file is part of:                           */
/*                           DungeonPath                                 */
/*             https://github.com/inthelamp/dungeonpath                  */
/*************************************************************************/
/* Copyright (c) 2018-2021 Dong Won Kim.                                 */
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
	//Set MaximumAP
    public override void SetMaximumAP(int level)
    {
        //The constant number can be considerd later to adjust the game balance        
		double results = (Constants.BaseHP +
		(Constants.MaximumPossibleHP - Constants.BaseHP) *
		level / Constants.MaximumLevel) / 10;
		MaximumAP = (int)Math.Floor(results);
    }

    //Set MinimumAP
    public override void SetMinimumAP(int level)
    {
        //The constant number can be considerd later to adjust the game balance        
		double results = (Constants.BaseHP +
		(Constants.MaximumPossibleHP - Constants.BaseHP) *
		level / Constants.MaximumLevel) / 15;
        MinimumAP = (int)Math.Floor(results);
    }

    //Random number between MinimumAP and MaximumAP
	public override int GetAttackPoints()
    {
        if (MaximumAP == 0) 
        {
            SetMaximumAP(Level);
        }
        if (MinimumAP == 0) 
        {
            SetMinimumAP(Level);
        }        
        return (int)Constants.RandRand(MinimumAP, MaximumAP);
    }
}
