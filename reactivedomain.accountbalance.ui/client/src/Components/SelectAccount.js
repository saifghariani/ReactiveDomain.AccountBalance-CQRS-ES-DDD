import React from 'react';


class SelectAccount extends React.Component{

    constructor(props){
        super(props);
        this.state = {
            accountId : '',
            holderName : ''
        };
    }
    handleSelect = (event)=>{
        event.preventDefault();
        this.props.onSubmit(this.state);
        
    }
    handleAdd = (event)=>{
        event.preventDefault();
        this.props.onAdd(this.state);
    }
    render(){
        return (
            <div className='panel'>
            <table style={{textAlign : 'right'}}>
                <tr>
                    <td><label for="accountId">AccountId</label></td>
                    <td><input name="accountId" type="text"  onChange={(event) => this.setState({ accountId: event.target.value })} placeholder="Leave empty if new"/></td>
                </tr>
                <tr>
                    <td><label for="holderName">HolderName</label></td>
                    <td><input name="holderName" type="text"  onChange={(event) => this.setState({ holderName: event.target.value })}/></td>
                </tr>
                <tr>
                    <td></td>
                    <td><button onClick={this.handleAdd} className='sign-button'>New</button><button onClick={this.handleSelect} className='sign-button'>Ok</button></td></tr>
            </table>
            </div>
        );
    }
}
export default SelectAccount;