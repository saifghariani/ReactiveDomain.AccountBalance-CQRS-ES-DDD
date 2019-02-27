import React from 'react'
import '../App.css'

const AccountDetails = (props)=>{
    return(
        <div>
            <table >
                <tr>
                    <td></td>
                    <td>State</td>
                    <td style={{color: props.account.State==="Active" ? 'green':'red'}}>{props.account.State}</td>
                    <td></td>
                </tr>
                <tr>
                    <td>Holder Name</td>
                    <td>: {props.account.HolderName}</td>
                    <td>Balance</td>
                    <td>: {props.account.Balance} $</td>
                </tr>
                <tr>
                    <td>Overdraft Limit</td>
                    <td>: {props.account.OverDraftLimit} $</td>
                    <td>DailyWireTransfer Limit</td>
                    <td>: {props.account.DailyWireTransferLimit} $</td>
                </tr>           
            </table>
        </div>
    );

}
export default AccountDetails;