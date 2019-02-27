import React from 'react';
import Account from './Account'

class AccountList extends React.Component{
   
    render(){
    return (
        <div >
            <h3>Account List</h3>
        <table border="2px" style={{border: '2px solid black'}}>
        
            <tbody><tr><th>AccountId</th><th>HolderName</th><th>S</th></tr>
            {this.props.accounts.map((account)=> <Account {...account}/>)}
            </tbody>
        </table>
        </div>
    );
    }
}
export default AccountList;