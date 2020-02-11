// use redis
// export default function(session, RedisStore, client){
  export default function(session){
  return session({ 
    // connect redis as a session storage
    // store: new RedisStore({client}),
    name:'__id',
    secret: process.env.NODE_ENV === 'production' ? process.env.SESSION_PASS : 'keyboard cat',
    cookie: {httpOnly: false, maxAge: 1000 * 60 * 60 * 24},
    resave: true,
    saveUninitialized: true
  })
}