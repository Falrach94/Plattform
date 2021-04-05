
/*
 * Connection States: 
 *      - Connecting: new ep 
 *      - Synchronizing: ep in storage
 *      - Established: ConnectionEstablished sent 
 *      - Disconnected: ep lost connection
 *      
 *      synchronization via connectionLock
 *      
 *      pulse by transition Synchronizing
 *
 *      valid transitions:
 *          - Connecting -> Synchronizing -> Established
 *          - Connecting, Synchronizing, Established -> Disconnected
 *      
 * 
 * Messages:
 * - server: ConnectionEstablished, Connections, Disconnect
 * - client: Ready
 * 
 * Connecting (event):
 *  . new Connection (State.Connecting)
 *  . insert Connection into storage
 *  . lock (storageLock)
 *  .   Connection.State = State.Synchronizing
 *  .   send Connections
 *  .   broadcast Connection
 *  . unlock
 *  . synchronize modules
 *  . send ConnectionEstablished
 *  . receive Ready -> fail to receive: disconnect ep, remove from storage
 *  . Connection.State = State.Established
 * 
 * Message Handler:
 * . lock (connectionLock)
 * .    if State == Disconnected || Connecting: abort
 * .    if State == Synchronizing && !Response: abort
 * .    process message
 * . unlock
 * 
 * Disconnect (event):
 *  . lock (connectionLock)
 *  .   while(Connection.State == Connecting)
 *  .      wait(connectionLock)
 *  . unlock
 *  . remove Connection from storage
 *  . lock (storageLock)
 *  .   broadcast Disconnect
 *  .   Connection.State = State.Disconnected
 *  . unlock
 * 
 * Disconnect
 * 
 */